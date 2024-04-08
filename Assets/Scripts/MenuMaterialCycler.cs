using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuMaterialCycler : MonoBehaviour
{
    public Transform[] elements;
    public Material replacementMaterial;

    public float waitTimeMin;
    public float waitTimeMax;
    public float holdMaterialTimeMin;
    public float holdMaterialTimeMax;
    public float repeatWaitTimeMin;
    public float repeatWaitTimeMax;

    public bool randomizeCycle;

    MeshRenderer[] renderers;
    Material[] originalMaterials;

    int changesCount;
    int index;
    float timer;

    enum State
    {
        holding,
        waiting,
        repeatWaiting
    }

    State state;

    // Start is called before the first frame update
    void Start()
    {
        renderers = new MeshRenderer[elements.Length];
        originalMaterials = new Material[elements.Length];
        for(int i = 0; i < elements.Length; i++)
        {
            MeshRenderer r = elements[i].GetComponent<MeshRenderer>();
            renderers[i] = r;
            originalMaterials[i] = r.material;

        }

        ChangeOrInitMaterial(true);

        state = State.holding;
        timer = UnityEngine.Random.Range(holdMaterialTimeMin, holdMaterialTimeMax);

    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;

        if (timer < 0)
        {
            if(state == State.waiting)
            {
                ChangeOrInitMaterial(false);
                timer = UnityEngine.Random.Range(holdMaterialTimeMin, holdMaterialTimeMax);
                state = State.holding;
            }
            else if(state == State.holding)
            {
                renderers[index].material = originalMaterials[index];

                if (changesCount < renderers.Length)
                {
                    timer = UnityEngine.Random.Range(waitTimeMin, waitTimeMax);
                    state = State.waiting;
                }
                else
                {
                    timer = UnityEngine.Random.Range(repeatWaitTimeMin, repeatWaitTimeMax);
                    state = State.repeatWaiting;
                }
            }
            else // state == State.repeatWait
            {
                changesCount = 0;
                ChangeOrInitMaterial(false);
                state = State.holding;
                timer = UnityEngine.Random.Range(holdMaterialTimeMin, holdMaterialTimeMax);

            }
        }

    }

    void ChangeOrInitMaterial(bool isInit)
    {
        if(!isInit) { renderers[index].material = originalMaterials[index]; }
        
        if(randomizeCycle) { index = UnityEngine.Random.Range(0, renderers.Length);  }
        else if(!isInit) { index = (index + 1) % renderers.Length; }

        renderers[index].material = replacementMaterial;

        changesCount ++;
    }

}
