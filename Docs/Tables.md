# Tables 配置表

基于Luban框架 ： [Luban Docs](https://www.datable.cn/docs/intro)

Tables 配置表用于定义和管理游戏中的各种数据表结构和属性。通过配置表，可以方便地创建、修改和删除数据表，并设置表的字段、数据类型、索引等信息。

## 特性

- luban是一个强大、易用、优雅、稳定的游戏配置解决方案。它设计目标为满足从小型到超大型游戏项目的简单到复杂的游戏配置工作流需求。

- luban可以处理丰富的文件类型，支持主流的语言，可以生成多种导出格式，支持丰富的数据检验功能，具有良好的跨平台能力，并且生成极快。

- luban有清晰优雅的生成管线设计，支持良好的模块化和插件化，方便开发者进行二次开发。开发者很容易就能将luban适配到自己的配置格式，定制出满足项目要求的强大的配置工具。

- luban标准化了游戏配置开发工作流，可以极大提升策划和程序的工作效率。

## 项目中使用

1. 配置表
   游戏中所有配置表放到GameConfigs下，根据Luban的配置要求进行组织和配置

2. 导出表
   在Editor工具中使用Luban导出表工具（Tools/导出Excel），将配置表导出为游戏运行时需要的格式
   导出的数据在Assets/LubanGenerated目录下

3. 加载表
   项目封装了基于Luban的配置表读取方法

## 使用\集成到Unity

1. **安装 com.code-philosophy.luban**

   在Package Manager中安装com.code-philosophy.luban包，地址:

    - `https://github.com/focus-creative-games/luban_unity.git`
    - [
      `https://gitee.com/focus-creative-games/luban_unity.git`](https://gitee.com/focus-creative-games/luban_unity.git) (
      备选)
    -

   > 网络代理问题可以用SSH地址代替HTTPS

>

2. **安装** [dotnet sdk 8.0](https://dotnet.microsoft.com/download/dotnet/8.0)
3. **下载[luban_examples项目](https://gitee.com/focus-creative-games/luban_examples)**
4. `luban_examples/Tools/Luban`
   目录下的Luban可能不是最新版本。开发者既可以从[release](https://github.com/focus-creative-games/luban/releases)
   直接下载Luban的最新版本，也可以自己从Luban源码编译。
5. **复制Luban工具到你的项目**

   大多数项目一般有一个专用目录放这些第三方的工具，比如说`{proj}/Tools`，将`luban_examples/Tools/Luban`复制到任意合适的目录即可。

6. **创建策划配置目录**

   将`luban_examples/MiniTemplate`复制到项目的合适位置，如`{proj}`
   。建议将MiniTemplate改名为DataTables或者别的名字，MiniTemplates下的子目录建议保持原名。


7. 修改luban.conf

每个项目的Luban工具的位置不同，需要修改`gen.bat`（或gen.sh）命令中Luban.dll的路径。假设你将MiniTemplate目录复制到了你项目的
`{DataTables}`目录。 打开`{DataTables}/gen.bat`，将`set LUBAN_DLL=%WORKSPACE%\Tools\Luban\Luban.dll`中
`%WORKSPACE%\Tools\Luban\Luban.dll`替换成实际的Luban.dll的目录。 Luban.dll在Luban工具目录下。

此时运行`{DataTables}/gen.bat`，确保可以正确运行。

8. 新增客户端和服务器的gen.bat脚本

   在客户端的合适位置创建 gen_client.bat脚本，内容大致如下：

    ```bash
    set GEN_CLIENT={Luban.dll的路径}
    set CONF_ROOT={DataTables目录的路径}
    
    dotnet %GEN_CLIENT% ^
        -t client ^
        -c cs-simple-json ^
        -d json ^
        --conf %CONF_ROOT%\luban.conf ^
        -x outputCodeDir={生成的代码的路径} ^
        -x outputDataDir={生成的数据的路径}
    ```

## 链接