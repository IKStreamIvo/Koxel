//-----------------------------------------------------------------------
// <copyright file="ThreadQueuer.cs" company="Quill18 Productions">
//     Copyright (c) Quill18 Productions. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;

namespace Koxel.Tech
{
    public class ThreadQueuer : MonoBehaviour
    {
        void Awake()
        {
            functionsToRunInMainThread = new List<Action>();
        }

        void Update()
        {
            // Update() always runs in the main thread

            while (functionsToRunInMainThread.Count > 0)
            {
                // Grab the first/oldest function in the list
                Action someFunc = functionsToRunInMainThread[0];
                functionsToRunInMainThread.RemoveAt(0);

                // Now run it
                someFunc();
            }
        }

        List<Action> functionsToRunInMainThread;

        public void StartThreadedFunction(Action someFunctionWithNoParams)
        {
            Thread t = new Thread(new ThreadStart(someFunctionWithNoParams));
            t.Start();
        }

        public void QueueMainThreadFunction(Action someFunction)
        {
            functionsToRunInMainThread.Add(someFunction);
        }
    }
}