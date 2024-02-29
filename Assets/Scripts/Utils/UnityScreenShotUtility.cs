#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Recorder;
using UnityEditor.Recorder.Input;
using UnityEngine;

public class UnityScreenShotUtility : MonoBehaviour
{
    private List<RecorderController> _recorderController = new();
    private List<(int, int)> _screenshotSizes = new();
    private void OnEnable()
    {
        _screenshotSizes = new()
        {
            //(2796, 1290),
            //(2778, 1284),
            //(2208, 1242),
            //(2732, 2048)

            (1290, 2796),
            (1284, 2778),
            (1242, 2208),
            (2048, 2732)
        };

        for (int i = 0; i < _screenshotSizes.Count; i++)
        {
            var lWidth = _screenshotSizes[i].Item1;
            var lHeight = _screenshotSizes[i].Item2;
            
            var lSettings = ScriptableObject.CreateInstance<RecorderControllerSettings>();
            _recorderController.Add(new RecorderController(lSettings));
            var lMediaOutputFolder = Path.Combine(Application.dataPath, "..", $"Recording/{lWidth}x{lHeight}");
            
            var lImageRecorder = ScriptableObject.CreateInstance<ImageRecorderSettings>();
            lImageRecorder.name = $"Image Recorder({lWidth}x{lHeight})";
            lImageRecorder.Enabled = true;
            lImageRecorder.OutputFormat = ImageRecorderSettings.ImageRecorderOutputFormat.PNG;
            lImageRecorder.CaptureAlpha = false;
            lImageRecorder.OutputFile =
                Path.Combine(lMediaOutputFolder, "image") + DefaultWildcard.Take;

            lImageRecorder.imageInputSettings = new GameViewInputSettings()
            {
                OutputWidth = lWidth,
                OutputHeight = lHeight,
            };

            lSettings.AddRecorderSettings(lImageRecorder);
            lSettings.SetRecordModeToSingleFrame(0);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(CaptureProcess());
        }
    }

    private IEnumerator CaptureProcess()
    {
        for (int i = 0; i < _recorderController.Count; i++)
        {
            _recorderController[i].PrepareRecording();
            _recorderController[i].StartRecording();

            while (_recorderController[i].IsRecording())
                yield return null;
        }
        
    }
}
#endif