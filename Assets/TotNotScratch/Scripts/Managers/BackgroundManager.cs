using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class BackgroundManager : PhoenixLikeSingleton<BackgroundManager>
{
    [SerializeField]
    private string BackgroundFolderResources = "Backgrounds";

    [SerializeField, Header("If none, searches self and main camera")]
    private SpriteRenderer _bgSpriteRenderer;
    private SpriteRenderer bgSpriteRenderer {
        get {
            if (!_bgSpriteRenderer) {
                _bgSpriteRenderer = GetComponent<SpriteRenderer>();
                if(!_bgSpriteRenderer) {
                    _bgSpriteRenderer = Camera.main.GetComponentInChildren<SpriteRenderer>();
                }
            }
            return _bgSpriteRenderer;
        }
    }

    public void setBackground(string backgroundName) {
        Sprite bg = Instantiate<Sprite>(Resources.Load<Sprite>(string.Format("{0}/{1}", BackgroundFolderResources, backgroundName)));
        bgSpriteRenderer.sprite = bg;
    }

    public void setColor(Color c) {
        bgSpriteRenderer.color = c;
    }
    
}
