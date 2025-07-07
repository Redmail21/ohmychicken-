using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GenericScripts 
{        
        // Function to return a random true or false value with custom odds
        public static bool GetRandomBool(float trueProbabilityDecimal)
        {
            float randomNumber = UnityEngine.Random.Range(0f, 1f);
            return randomNumber < trueProbabilityDecimal;
        }


    
    
        
    


    public class FunctionTimer
    {
            private static List<FunctionTimer> activeTimersList;
            private static GameObject initGameObject;

            private static void InitIfNeeded()
            {
                if (initGameObject == null)
                {
                    initGameObject = new GameObject("FunctionTimer_InitGameObject");
                    activeTimersList = new List<FunctionTimer>();

                }
            }




            public static FunctionTimer Create(Action action, float timer, string timerName = null){
                InitIfNeeded();

                GameObject gameobject = new GameObject("FunctionTimer", typeof(MonoBehaviourHook));
                FunctionTimer functionTimer = new FunctionTimer(action, timer, gameobject, timerName);

                gameobject.GetComponent<MonoBehaviourHook>().onUpdate = functionTimer.Update;
            
                activeTimersList.Add(functionTimer);

                return functionTimer;
            }    

            private static void StopTimer(string timerName)
            {
                for(int i = 0; i < activeTimersList.Count; i++)
                {

                if (activeTimersList[i].timerName == timerName)
                {
                    activeTimersList[i].DestroySelf();
                    i--;
                }

                }
            }

            private static void RemoveTimer(FunctionTimer functionTimer)
            {
                InitIfNeeded();
                activeTimersList.Remove(functionTimer);
            }

            public class MonoBehaviourHook : MonoBehaviour {

                public Action onUpdate;
                private void Update()
                {
                    if(onUpdate != null) onUpdate();
                }

            }

    
            

            private Action action;
            private float timer;
            private bool isDestroyed;
            private GameObject gameObject;
            private string timerName;

            
            public FunctionTimer(Action action, float timer, GameObject gameObject, string timerName)
        {

                this.action = action;
                this.timer = timer;
                this.gameObject = gameObject;
                this.timerName = timerName;
                isDestroyed = false;

            }   
        
            public void Update()
            {
                this.timer -= Time.deltaTime;
                if (this.timer <= 0f)
                {
                    //Do stuff
                    Debug.Log("Triggering Action");
                    action();
                    DestroySelf();

                }

            }

            private void DestroySelf(){

                isDestroyed = true;
                UnityEngine.Object.Destroy(gameObject);
                RemoveTimer(this);  
            }

        }//End of FunctionTimer



    }//End of main class
