using UnityEngine;
using System.Collections;

public class WFX_Demo_DeleteAfterDelay : MonoBehaviour
{
	[SerializeField] private float delay;
	
	void Update()
	{
		delay -= Time.deltaTime;

		if(delay < 0f) gameObject.SetActive(false);
	}
}
