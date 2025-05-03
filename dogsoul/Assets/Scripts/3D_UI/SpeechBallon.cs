using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using static System.Net.Mime.MediaTypeNames;

public class SpeechBallon : MonoBehaviour
{
    
    [SerializeField] string textContent;
    [SerializeField] TextMeshProUGUI text;


    public float scaleSpeed = 2f;
    public Vector3 initialScale;
    private Vector3 stepScale = new Vector3(0.015f, 0.015f, 0.015f);
    private Vector3 targetScale;
    private bool isScaligUp = false;

    public Vector3 plusPosition;
    
    
    private void OnEnable()
    {
        Canvas canvas = GetComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main; //일단 main 카메라
        
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * scaleSpeed);
        transform.position = transform.parent.position + plusPosition;
        if (Vector3.Distance(transform.localScale, targetScale) < 0.0001f)
        {
            transform.localScale = targetScale;
        }
        if(Vector3.Distance(transform.localScale, Vector3.zero) < 0.0001f)
        {
            gameObject.SetActive(false);
        }

        transform.rotation = Camera.main.transform.rotation;
    }

    

    public void SetActiveTrue()
    {
        transform.localScale = Vector3.zero;
        gameObject.SetActive(true);
        text.text = textContent;
        targetScale = stepScale;
    }
    public void SetActiveFalse()
    {
        targetScale = Vector3.zero;
    }
    
}
