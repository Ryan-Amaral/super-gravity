using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuiHealth : MonoBehaviour {

  public Health healthObj;

  private RectTransform rt;
  private float width;

	// Use this for initialization
	void Start () {
    rt = GetComponent<RectTransform>();
    width = rt.rect.width;
	}

	// Update is called once per frame
	void Update () {
		rt.sizeDelta = new Vector2(
                Mathf.Max(width * (healthObj.curHealth / healthObj.maxHealth), 0),
                rt.rect.height);
	}
}
