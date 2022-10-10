using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;
public class JobsDemoScript : MonoBehaviour
{
    [SerializeField] private bool useJobsSystem;

    [SerializeField] private Transform Pfplants;

    private List<Zombie> plantList;
    
    
    public class Zombie
    {
        public Transform transform;
        public float moveY;
    }

    private void Start()
    {
        plantList = new List<Zombie>();
        for (int i = 0; i < 1000; i++)
        {
            Transform zombieTransform = Instantiate(Pfplants, new Vector3(UnityEngine.Random.Range(-10f, 10f), UnityEngine.Random.Range(-10f, 10f)), Quaternion.identity);
        }
        
    }
    private void Update()
      {
        float startTime = Time.realtimeSinceStartup;
        if (useJobsSystem)
        {
            NativeList<JobHandle> jobs = new NativeList<JobHandle>(Allocator.Temp);
            for (int i = 0; i < 10; i++)
            {
                
                JobHandle jobHandle = TaskJob();
                jobs.Add(jobHandle);

            }
            JobHandle.CompleteAll(jobs);
            jobs.Dispose();
               // newjob.Complete();
        }
        else
        {
            for (int i = 0; i < 10; i++)
            {
                ExampleJob();
            }
        }
        Debug.Log((Time.realtimeSinceStartup - startTime) * 1000f + "ms");

    }

    public void ExampleJob()
    {
        float value = 0f;
        for (int i = 0; i < 99999; i++)
        {
            value = math.exp10(math.sqrt(value));
        }

    }
    private JobHandle TaskJob()
    {
        JobStruct jobs = new JobStruct();
        return jobs.Schedule();
    }

    [BurstCompile]
    public struct JobStruct : IJob
    {
        public void Execute()
        {
            float value = 0f;
            for (int i = 0; i < 99999; i++)
            {
                value = math.exp10(math.sqrt(value));
            }
        }
    }

}
