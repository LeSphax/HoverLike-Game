using MLAgents;
using UnityEngine;
using UTJ.FrameCapturer;

[RequireComponent(typeof(MovieRecorder))]
public class RecordVideos : MonoBehaviour
{

    public int IntervalInAcademySteps = -1;
    public int DurationInAcademySteps = -1;

    private Academy academy;
    private bool isRecording = false;
    private int recordingStartStep = -1;
    private int steps = 0;


    private MovieRecorder movieRecorder;

    private void Awake()
    {
        movieRecorder = GetComponent<MovieRecorder>();
        academy = FindObjectOfType<Academy>();
    }

    private void Start()
    {
        if (IntervalInAcademySteps < DurationInAcademySteps)
        {
            Debug.LogError("The duration of a video should be smaller than the interval");
        }
    }


    private void FixedUpdate()
    {
        if (IntervalInAcademySteps != -1 && DurationInAcademySteps != -1)
        {
            if (isRecording && steps - recordingStartStep > DurationInAcademySteps)
            {
                movieRecorder.EndRecording();
                isRecording = false;
            }
            // Order is important in case we want to begin a recording on the same frame we ended one
            else if (!isRecording && steps % IntervalInAcademySteps == 0)
            {
                movieRecorder.BeginRecording();
                isRecording = true;
                recordingStartStep = steps;
            }
        }
        steps++;
    }

}
