# MVC框架详细使用说明

## 快速开始

### 1. 基础MVC组合
```csharp
// 创建模型
var model = new SimpleModel<float>(100f);

// 创建视图（继承BaseView）
var view = gameObject.AddComponent<MyView>();

// 创建控制器（继承BaseController）
var controller = new MyController();
controller.Initialize(model, view);
controller.Start();

// 注册到管理器
MVCManager.Instance.RegisterController(controller);
```

### 2. UI栈管理（推荐）
```csharp
// 创建UI控制器
[UILayer(UILayer.Popup, blockInput: true, allowStack: true)]
public class SettingsUIController : BaseUIController<SettingsModel, SettingsView>
{
    // 实现UI逻辑
}

// 使用
MVCManager.Instance.Open<SettingsUIController>(openArgs);
MVCManager.Instance.Close<SettingsUIController>();
MVCManager.Instance.CloseTop();
```

## 核心组件详解

### Model - 数据层

#### SimpleModel<T> - 简单数据模型
```csharp
var healthModel = new SimpleModel<float>(100f);
healthModel.OnValueChanged += (newValue) => Debug.Log($"Health: {newValue}");
healthModel.SetValue(80f); // 触发事件
```

#### EventDrivenModel<T, TEvent> - 事件驱动模型
```csharp
public class HealthModel : EventDrivenModel<float, HealthChangedEvent>
{
    protected override HealthChangedEvent CreateEvent(float oldValue, float newValue)
    {
        return new HealthChangedEvent(oldValue, newValue);
    }
    
    public void TakeDamage(float damage)
    {
        SetValue(Mathf.Max(0, GetValue() - damage));
    }
}
```

#### 自定义模型
```csharp
public class PlayerStatsModel : BaseModel<PlayerStats>
{
    public void LevelUp()
    {
        var stats = Value;
        stats.Level++;
        stats.Experience = 0;
        SetValue(stats);
    }
    
    public void GainExp(int exp)
    {
        var stats = Value;
        stats.Experience += exp;
        if (stats.Experience >= stats.MaxExperience)
        {
            LevelUp();
        }
        else
        {
            SetValue(stats);
        }
    }
}
```

### View - 视图层

#### 基础视图实现
```csharp
public class HealthBarView : BaseView<float>
{
    [SerializeField] private Image healthBar;
    [SerializeField] private Text healthText;
    
    public override void UpdateView(float health)
    {
        healthBar.fillAmount = health / 100f;
        healthText.text = $"{health:F0}/100";
    }
    
    protected override void OnInitialize()
    {
        // 视图初始化逻辑
        base.OnInitialize();
    }
}
```

#### 复合视图
```csharp
public class PlayerStatsView : BaseView<PlayerStats>
{
    [SerializeField] private Text levelText;
    [SerializeField] private Image expBar;
    [SerializeField] private Text expText;
    
    public override void UpdateView(PlayerStats stats)
    {
        levelText.text = $"Level {stats.Level}";
        expBar.fillAmount = (float)stats.Experience / stats.MaxExperience;
        expText.text = $"{stats.Experience}/{stats.MaxExperience}";
    }
}
```

### Controller - 控制层

#### 传统MVC控制器
```csharp
public class HealthController : BaseController<HealthModel, HealthBarView>
{
    protected override void OnInitialize()
    {
        View.BindModel(Model);
    }
    
    protected override void OnStart()
    {
        // 监听游戏事件
        EventManager.Instance.Subscribe<DamageEvent>(OnDamageReceived);
    }
    
    protected override void OnStop()
    {
        EventManager.Instance.Unsubscribe<DamageEvent>(OnDamageReceived);
    }
    
    private void OnDamageReceived(DamageEvent damageEvent)
    {
        Model.TakeDamage(damageEvent.Amount);
    }
}
```

#### UI栈控制器
```csharp
[UILayer(UILayer.Window, blockInput: false, allowStack: false)]
public class InventoryUIController : BaseUIController<InventoryModel, InventoryView>
{
    protected override void OnBeforeOpen(object args)
    {
        // 打开前逻辑：加载数据
        if (args is InventoryOpenArgs openArgs)
        {
            Model.LoadInventory(openArgs.PlayerId);
        }
    }

    protected override void OnAfterOpen(object args)
    {
        // 打开后逻辑：播放动画
        View.PlayOpenAnimation();
    }
    
    protected override void OnBeforeClose()
    {
        // 关闭前逻辑：保存状态
        Model.SaveInventory();
    }
}
```

## 实际应用案例

### 案例1：英雄血量系统
```csharp
// 1. 数据结构
[System.Serializable]
public struct HeroHealthData
{
    public float CurrentHealth;
    public float MaxHealth;
    public float HealthPercentage => MaxHealth > 0 ? CurrentHealth / MaxHealth : 0;
}

// 2. 模型
public class HeroHealthModel : BaseModel<HeroHealthData>
{
    public void SetMaxHealth(float maxHealth)
    {
        var data = Value;
        data.MaxHealth = maxHealth;
        data.CurrentHealth = Mathf.Min(data.CurrentHealth, maxHealth);
        SetValue(data);
    }
    
    public void SetCurrentHealth(float currentHealth)
    {
        var data = Value;
        data.CurrentHealth = Mathf.Clamp(currentHealth, 0, data.MaxHealth);
        SetValue(data);
    }
}

// 3. 视图
public class HeroHpBarView : BaseView<HeroHealthData>
{
    [SerializeField] private Image barImage;
    [SerializeField] private Text healthText;
    
    public override void UpdateView(HeroHealthData data)
    {
        barImage.fillAmount = data.HealthPercentage;
        healthText.text = $"{data.CurrentHealth:F0}/{data.MaxHealth:F0}";
        
        // 血量不足时变色
        if (data.HealthPercentage < 0.3f)
            barImage.color = Color.red;
        else if (data.HealthPercentage < 0.6f)
            barImage.color = Color.yellow;
        else
            barImage.color = Color.green;
    }
}

// 4. 控制器
public class HeroHealthController : BaseController<HeroHealthModel, HeroHpBarView>
{
    [SerializeField] private UnitComponent unit;
    
    protected override void OnStart()
    {
        // 监听单位血量变化
        if (unit != null)
        {
            unit.OnHealthChanged += OnUnitHealthChanged;
            // 初始化血量
            Model.SetMaxHealth(unit.MaxHealth);
            Model.SetCurrentHealth(unit.CurrentHealth);
        }
    }
    
    private void OnUnitHealthChanged(float currentHealth, float maxHealth)
    {
        Model.SetMaxHealth(maxHealth);
        Model.SetCurrentHealth(currentHealth);
    }
    
    protected override void OnStop()
    {
        if (unit != null)
        {
            unit.OnHealthChanged -= OnUnitHealthChanged;
        }
    }
}

// 5. 一体化组件
public class HeroHpBarMVC : MonoBehaviour
{
    private HeroHealthController _controller;
    
    void Start()
    {
        var model = new HeroHealthModel();
        var view = GetComponent<HeroHpBarView>();
        
        _controller = new HeroHealthController();
        _controller.Initialize(model, view);
        MVCManager.Instance.RegisterController(_controller);
        _controller.Start();
    }
    
    void OnDestroy()
    {
        if (_controller != null)
        {
            MVCManager.Instance.UnregisterController(_controller);
        }
    }
}
```

### 案例2：主菜单UI系统
```csharp
// 1. 主菜单模型
public class MainMenuModel
{
    public string PlayerName { get; set; }
    public int PlayerLevel { get; set; }
    public int Currency { get; set; }
    public bool IsGameInProgress { get; set; }
}

// 2. 主菜单视图
public class MainMenuView : MonoBehaviour
{
    [Header("UI Components")]
    public Text playerNameText;
    public Text playerLevelText;
    public Text currencyText;
    public Button continueButton;
    public Button newGameButton;
    public Button settingsButton;
    public Button exitButton;
    
    public void UpdateView(MainMenuModel model)
    {
        playerNameText.text = model.PlayerName;
        playerLevelText.text = $"Level {model.PlayerLevel}";
        currencyText.text = $"{model.Currency} Coins";
        continueButton.interactable = model.IsGameInProgress;
    }
    
    private void Start()
    {
        continueButton.onClick.AddListener(OnContinueClicked);
        newGameButton.onClick.AddListener(OnNewGameClicked);
        settingsButton.onClick.AddListener(OnSettingsClicked);
        exitButton.onClick.AddListener(OnExitClicked);
    }
    
    private void OnContinueClicked()
    {
        // 通过事件通知控制器
        EventManager.Instance.Publish(new ContinueGameEvent());
    }
    
    private void OnNewGameClicked()
    {
        EventManager.Instance.Publish(new NewGameEvent());
    }
    
    private void OnSettingsClicked()
    {
        MVCManager.Instance.Open<SettingsUIController>();
    }
    
    private void OnExitClicked()
    {
        Application.Quit();
    }
}

// 3. 主菜单控制器
[UILayer(UILayer.Window, blockInput: false, allowStack: false)]
public class MainMenuUIController : BaseUIController<MainMenuModel, MainMenuView>
{
    protected override void OnAfterOpen(object args)
    {
        // 加载玩家数据
        LoadPlayerData();
    }
    
    private void LoadPlayerData()
    {
        // 从存档加载玩家数据
        var playerData = SaveManager.LoadPlayerData();
        Model.PlayerName = playerData.Name;
        Model.PlayerLevel = playerData.Level;
        Model.Currency = playerData.Currency;
        Model.IsGameInProgress = SaveManager.HasSaveGame();
        
        View.UpdateView(Model);
    }
    
    protected override void OnStart()
    {
        EventManager.Instance.Subscribe<ContinueGameEvent>(OnContinueGame);
        EventManager.Instance.Subscribe<NewGameEvent>(OnNewGame);
    }
    
    protected override void OnStop()
    {
        EventManager.Instance.Unsubscribe<ContinueGameEvent>(OnContinueGame);
        EventManager.Instance.Unsubscribe<NewGameEvent>(OnNewGame);
    }
    
    private void OnContinueGame(ContinueGameEvent evt)
    {
        Close();
        GameManager.Instance.LoadGame();
    }
    
    private void OnNewGame(NewGameEvent evt)
    {
        Close();
        GameManager.Instance.StartNewGame();
    }
}
```

## 性能优化技巧

### 1. 批量更新
```csharp
// 避免频繁触发事件
model.OnValueChanged -= HandleValueChange;
model.SetValue(value1);
model.SetValue(value2);
model.SetValue(value3);
model.OnValueChanged += HandleValueChange;
model.SetValue(finalValue); // 只触发一次更新
```

### 2. 更新频率控制
```csharp
public class HealthBarView : BaseView<float>
{
    [SerializeField] private float updateInterval = 0.1f;
    private float lastUpdateTime;
    
    public override void UpdateView(float health)
    {
        if (Time.time - lastUpdateTime < updateInterval)
            return;
            
        lastUpdateTime = Time.time;
        // 执行实际更新
        healthBar.fillAmount = health / 100f;
    }
}
```

### 3. 手动控制更新
```csharp
// 禁用自动更新
view.SetAutoUpdate(false);

// 在需要时手动刷新
void LateUpdate()
{
    if (needsUpdate)
    {
        view.RefreshView();
        needsUpdate = false;
    }
}
```

## 调试和故障排除

### 调试技巧
```csharp
// 启用调试日志
MVCManager.Instance.SetDebugLogging(true);
controller.SetDebugLogging(true);

// 检查MVC状态
Debug.Log($"Controller Ready: {controller.IsControllerReady()}");
Debug.Log($"View Bound: {view.Model != null}");
Debug.Log($"Model Value: {model.GetValue()}");

// 获取统计信息
Debug.Log(MVCManager.Instance.GetStatistics());
Debug.Log(MVCManager.Instance.GetUIStackInfo());
```

### 常见问题解决

1. **视图不更新**
   - 检查Model是否正确绑定：`view.Model != null`
   - 确认AutoUpdate是否启用：`view.SetAutoUpdate(true)`
   - 验证事件订阅：检查OnValueChanged事件

2. **内存泄漏**
   - 确保调用Dispose()：`controller.Dispose()`
   - 检查事件订阅：正确取消订阅事件
   - 使用MVCManager管理：`MVCManager.Instance.RegisterController()`

3. **UI栈问题**
   - 检查层级配置：确认UILayerAttribute设置正确
   - 验证栈状态：`MVCManager.Instance.GetUIStackInfo()`
   - 确认输入阻塞：`MVCManager.Instance.HasInputBlocker()`

