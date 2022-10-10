using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;
using Unity.Collections;
using Unity.Burst;

public class JobsTesting : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        NativeList<JobHandle> jobHandlesList = new NativeList<JobHandle>(Allocator.Temp);

        JobHandle jobHandle = ReallyToughTaskJob();

        jobHandlesList.Add(jobHandle);
        //jobHandle.Complete();
        JobHandle.CompleteAll(jobHandlesList);
        jobHandlesList.Dispose();
    }

    JobHandle ReallyToughTaskJob()
    {
        ReallyToughJob job = new ReallyToughJob();
        return job.Schedule();
    }
}

[BurstCompile]
public struct ReallyToughJob : IJob
{
    public void Execute()
    {
        //Do something here..
    }
}
