using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Jobs;
public class JobsDemoScript : MonoBehaviour
{
    [SerializeField] private bool useJobsSystem;


    private void Update()
      {
        float startTime = Time.realtimeSinceStartup;
        if (useJobsSystem)
        {
            JobHandle newjob = JobSystemHandler();
            newjob.Complete();

        }
        else
        {
            ExampleJob();

        }
        
        
        Debug.Log((Time.realtimeSinceStartup - startTime) * 1000f + "ms");

    }

    public void ExampleJob()
    {
        float value = 0f;
        for (int i = 0; i < 999999; i++)
        {
            value = math.exp10(math.sqrt(value));
        }

    }
    private JobHandle JobSystemHandler()
    {
        JobStruct jobs = new JobStruct();
        return jobs.Schedule();
    }
    public struct JobStruct : IJob
    {

        public void Execute()
        {
            float value = 0f;
            for (int i = 0; i < 999999; i++)
            {
                value = math.exp10(math.sqrt(value));
            }
        }
    }

}
