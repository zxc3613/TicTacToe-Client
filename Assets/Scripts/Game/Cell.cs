using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MarkerType {None, Circle, Cross}
public class Cell : MonoBehaviour
{
    [SerializeField] SpriteRenderer MarkerSpriteRenderer;

    [SerializeField] Sprite circleMarkerSprite;
    [SerializeField] Sprite crossMarkerSprite;

    public int index;

    MarkerType markerType;
    public MarkerType MarkerType
    {
        get { return markerType; }
        set 
        {
            switch (value)
            {
                case MarkerType.None:
                    MarkerSpriteRenderer.sprite = null;
                    break;
                case MarkerType.Circle:
                    MarkerSpriteRenderer.sprite = circleMarkerSprite;
                    break;
                case MarkerType.Cross:
                    MarkerSpriteRenderer.sprite = crossMarkerSprite;
                    break;
            }
            markerType = value;
        }
    }
}
