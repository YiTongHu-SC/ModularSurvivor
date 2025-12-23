using System.Collections;
using Core.Coordinates;
using Core.Units;
using Lean.Pool;
using UnityEngine;

namespace Combat.Views.UnitViews
{
    public class LaserStrikeView : BaseUnitPresentation
    {
        public LineRenderer LineRenderer;
        private ViewBaseData _laserStrikeViewData;

        public override void SetViewData(ViewBaseData viewData)
        {
            base.SetViewData(viewData);
            _laserStrikeViewData = viewData;
        }

        public override void Apply()
        {
            // 实现激光打击的视觉效果
            // 例如，播放激光发射动画，处理延迟和持续时间等
            if (_laserStrikeViewData == null)
            {
                Remove();
                return;
            }

            StartCoroutine(PlayLaserStrikeEffect(_laserStrikeViewData.Delay, _laserStrikeViewData.Duration));
        }

        /// <summary>
        /// 播放激光打击效果的协程
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        private IEnumerator PlayLaserStrikeEffect(float delay, float duration)
        {
            var timer = 0f;
            yield return new WaitForSeconds(delay);
            CreateLaserEffect();
            while (timer < duration)
            {
                timer += Time.deltaTime;
                UpdateLaserEffect();
                yield return null;
            }

            Remove();
        }

        public override void OnSpawn()
        {
            base.OnSpawn();
            LineRenderer.gameObject.SetActive(false);
        }

        /// <summary>
        /// 创建激光效果
        /// </summary>
        private void CreateLaserEffect()
        {
            LineRenderer.gameObject.SetActive(true);
        }

        /// <summary>
        /// 更新激光效果,每帧调用
        /// 调整激光位置，始终指向目标
        /// </summary>
        private void UpdateLaserEffect()
        {
            LineRenderer.positionCount = 2;
            var getUnitData = UnitManager.Instance.Units.TryGetValue(ViewData.SourceId, out var unit);
            var getTargetData = UnitManager.Instance.Units.TryGetValue(ViewData.TargetIds[0], out var target);
            // 单位有可能在激光持续时间内被移除
            // 如果任一单位数据不存在，直接返回
            if (!getUnitData || !getTargetData)
            {
                return;
            }

            var originPosition =
                CoordinateConverter.ToWorldPosition(unit.Position, Vector3.up * unit.ModelView.CenterOffset);
            var targetPosition =
                CoordinateConverter.ToWorldPosition(target.Position, Vector3.up * unit.ModelView.CenterOffset);
            var direction = (targetPosition - originPosition).normalized;
            originPosition += direction * unit.ModelView.Radius; // 激光起点稍微前移
            targetPosition -= direction * unit.ModelView.Radius; // 激光终点稍微后移
            LineRenderer.SetPosition(0, originPosition);
            LineRenderer.SetPosition(1, targetPosition);
        }
    }
}