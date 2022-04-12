using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DispCommandView : MonoBehaviour
{
	[SerializeField] Image _image = null;

	[SerializeField] List<Sprite> _spriteList = null;

	public void SetCommandSprite(Commands commands)
	{
		Sprite sprite = null;
		switch (commands)
		{
			case Commands.Gu:
				sprite = _spriteList[0];
				break;
			case Commands.Choki:
				sprite = _spriteList[1];
				break;
			case Commands.Pa:
				sprite = _spriteList[2];
				break;
		}
		_image.sprite = sprite;
	}

	public void ShowDown()
	{
		gameObject.SetActive(true);
	}

	public void Reset()
	{
		_image.sprite = null;
		gameObject.SetActive(false);
	}
}
