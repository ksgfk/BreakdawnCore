using UnityEngine;
using UnityEngine.UI;

namespace Breakdawn.Unity
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Breakdawn/DisplayFPS")]
    public class DisplayFPS : MonoBehaviour
    {
        [Tooltip("用于显示FPS的Text组件")] public Text displayText;
        [Tooltip("设置测量间隔(单位:秒)")] public float fpsMeasuringDelta = 1;
        private float _timePassed;
        private int _frameCount;
        private float _fps;

        private void Update()
        {
            _frameCount += 1;
            _timePassed += Time.deltaTime;

            if (!(_timePassed > fpsMeasuringDelta))
            {
                return;
            }

            _fps = _frameCount / _timePassed;
            _timePassed = 0.0f;
            _frameCount = 0;
            displayText.text = ((int) _fps).ToString();
        }
    }
}