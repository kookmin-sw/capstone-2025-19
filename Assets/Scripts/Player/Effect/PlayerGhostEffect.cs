using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGhostEffect : MonoBehaviour
{
    [SerializeField]
    Material afterImageMaterial;

    SkinnedMeshRenderer smr;
    Afterimage[] afterImages;
    
    int afterImageCount;
    int currentAfterImageIndex;
    float remainAfterImageTime;
    float createAfterImagedelay;
    Coroutine createAfterImageCoroutine = null;

    bool isCreating = false;

    public void Setup(SkinnedMeshRenderer smr, int maxNumber, float remainTime)
    {
        this.smr = smr;
        afterImageCount = maxNumber;
        remainAfterImageTime = remainTime;
        createAfterImagedelay = remainAfterImageTime / (float)afterImageCount + 0.05f;

        CreateAfterImages();
    }

    void CreateAfterImages()
    {
        afterImages = new Afterimage[afterImageCount];
        for (int i = 0; i < afterImages.Length; ++i)
        {
            GameObject newObj = new GameObject();
            afterImages[i] = newObj.AddComponent<Afterimage>();
            afterImages[i].InitAfterImage(afterImageMaterial);
        }
    }

    public void Create(bool flag)
    {
        isCreating = flag;
        if ( flag )
        {
            if ( createAfterImageCoroutine == null )
                createAfterImageCoroutine = StartCoroutine(CreateAfterImageCoroution());
        }
    }

    IEnumerator CreateAfterImageCoroution()
    {
        float t = 0f;
        while ( isCreating )
        {
            t += Time.deltaTime;

            if (t >= createAfterImagedelay)
            {
                smr.BakeMesh(afterImages[currentAfterImageIndex].mesh);
                afterImages[currentAfterImageIndex].CreateAfterImage(transform.position, transform.rotation, remainAfterImageTime);
                currentAfterImageIndex = (currentAfterImageIndex + 1) % afterImageCount;
                t -= createAfterImagedelay;
            }
            yield return null;
        }

        createAfterImageCoroutine = null;
    }
}