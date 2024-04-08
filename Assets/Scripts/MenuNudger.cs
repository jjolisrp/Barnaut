using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuNudger : MonoBehaviour
{
    public enum Type
    {
        animatorParameter,
        cameraFOV,
        cameraOrthographicSize,
        transformPosition,
        transformRotation,
    };

    [System.Serializable]
    public struct NudgeProperties
    {
        public Type type;

        public float nudgeWaitTimeMin;
        public float nudgeWaitTimeMax;

        public float nudgeDurationTimeMin;
        public float nudgeDurationTimeMax;

        public float nudgeAmountMin;
        public float nudgeAmountMax;
        public AnimationCurve nudgeAmountFilter;

        public bool startNudging;

        public string animatorParameter;

    }

    public NudgeProperties[] nudges;

    struct NudgeState
    {
        public bool nudging;
        public float timer;
        public float nudgeDuration;
        public float nudgeAmount;
        public Vector3 nudgeDirection;
        public float waitDuration;

        public float originalValue;
        public Vector3 originalVectorValue;
        public Animator animator;
        public Camera camera;
    }

    NudgeState[] nudgeStates;

    // Start is called before the first frame update
    void Start()
    {
        nudgeStates = new NudgeState[nudges.Length];

        for(int i = 0; i < nudges.Length; i++)
        {
            if(nudges[i].type == Type.animatorParameter)
            {
                nudgeStates[i].animator = GetComponent<Animator>();
                nudgeStates[i].originalValue = nudgeStates[i].animator.GetFloat(nudges[i].animatorParameter);
            }
            else if(nudges[i].type == Type.transformPosition)
            {
                nudgeStates[i].originalVectorValue = transform.position;
            }
            else if (nudges[i].type == Type.transformRotation)
            {
                nudgeStates[i].originalVectorValue = transform.rotation.eulerAngles;
            }
            else // nudges[i].type == Type.cameraFOV || nudges[i].type == Type.orthographicSize
            {
                Camera c = GetComponent<Camera>();
                nudgeStates[i].camera = c;
                nudgeStates[i].originalValue = (nudges[i].type == Type.cameraFOV ? c.fieldOfView : c.orthographicSize);
            }

            if (nudges[i].startNudging)
            {
                nudgeStates[i].nudgeDuration = UnityEngine.Random.Range(nudges[i].nudgeDurationTimeMin, nudges[i].nudgeDurationTimeMax);
                nudgeStates[i].nudgeAmount = UnityEngine.Random.Range(nudges[i].nudgeAmountMin, nudges[i].nudgeAmountMax);
                nudgeStates[i].timer = 0;
                nudgeStates[i].nudging = true;
            }
            else
            {
                nudgeStates[i].waitDuration = UnityEngine.Random.Range(nudges[i].nudgeWaitTimeMin, nudges[i].nudgeWaitTimeMax);
                nudgeStates[i].timer = 0;
                nudgeStates[i].nudging = false;
            }

        }

        

    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < nudges.Length; i++)
        {
            nudgeStates[i].timer += Time.deltaTime;

            if(nudgeStates[i].nudging)
            {
                if(nudgeStates[i].timer >= nudgeStates[i].nudgeDuration)
                {
                    if(nudges[i].type == Type.animatorParameter)
                    {
                        nudgeStates[i].animator.SetFloat(nudges[i].animatorParameter, nudgeStates[i].originalValue);
                    }
                    else if (nudges[i].type == Type.transformPosition)
                    {
                        transform.position = nudgeStates[i].originalVectorValue;
                    }
                    else if (nudges[i].type == Type.transformRotation)
                    {
                        transform.rotation = Quaternion.Euler(nudgeStates[i].originalVectorValue);
                    }
                    else if(nudges[i].type == Type.cameraFOV)
                    {
                        nudgeStates[i].camera.fieldOfView = nudgeStates[i].originalValue;
                    }
                    else // orthographicSize
                    {
                        nudgeStates[i].camera.orthographicSize = nudgeStates[i].originalValue;
                    }

                    nudgeStates[i].waitDuration = UnityEngine.Random.Range(nudges[i].nudgeWaitTimeMin, nudges[i].nudgeWaitTimeMax);
                    nudgeStates[i].timer = 0;
                    nudgeStates[i].nudging = false;
                }
                else
                {
                    float nudgeAmount = nudges[i].nudgeAmountFilter.Evaluate(nudgeStates[i].timer / nudgeStates[i].nudgeDuration) * nudgeStates[i].nudgeAmount;

                    if(nudges[i].type == Type.animatorParameter)
                    {
                        nudgeStates[i].animator.SetFloat(nudges[i].animatorParameter, nudgeAmount + nudgeStates[i].originalValue);
                    }
                    else if(nudges[i].type == Type.transformPosition)
                    {
                        transform.position = nudgeStates[i].originalVectorValue + nudgeAmount * nudgeStates[i].nudgeDirection;
                    }
                    else if(nudges[i].type == Type.transformRotation)
                    {
                        transform.rotation = Quaternion.Euler(nudgeStates[i].originalVectorValue) * Quaternion.AngleAxis(nudgeAmount, nudgeStates[i].nudgeDirection);
                    }
                    else if(nudges[i].type == Type.cameraFOV)
                    {
                        nudgeStates[i].camera.fieldOfView = nudgeAmount + nudgeStates[i].originalValue;
                    }
                    else // orthographicSize
                    {
                        nudgeStates[i].camera.orthographicSize = nudgeAmount + nudgeStates[i].originalValue;
                    }
                }
            }
            else
            {
                if(nudgeStates[i].timer >= nudgeStates[i].waitDuration)
                {
                    if(nudges[i].type == Type.transformPosition || nudges[i].type == Type.transformRotation)
                    {
                        nudgeStates[i].nudgeDirection = UnityEngine.Random.onUnitSphere;
                    }


                    nudgeStates[i].nudgeDuration = UnityEngine.Random.Range(nudges[i].nudgeDurationTimeMin, nudges[i].nudgeDurationTimeMax);
                    nudgeStates[i].nudgeAmount = UnityEngine.Random.Range(nudges[i].nudgeAmountMin, nudges[i].nudgeAmountMax);
                    nudgeStates[i].timer = 0;
                    nudgeStates[i].nudging = true;
                }
            }
        }
        
    }
}
