using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RaymarchingToolkit.Examples {
[RequireComponent(typeof(Button))]
public class ColorPicker : XYControl {

	[SerializeField] RawImage m_ColorImage;
	[SerializeField] Texture2D m_CustomPalette;

	[System.Serializable]
	public class ColorPickerEvent : UnityEvent<Color> { }
	public ColorPickerEvent onColorChanged = new ColorPickerEvent();

	Texture2D m_ColorTexture;
	Color m_Color;
	const int SIZE = 10;

	void Start() {
		if (m_CustomPalette) {
			m_ColorTexture = m_CustomPalette;
		} else {
			m_ColorTexture = new Texture2D(SIZE, SIZE);
			m_ColorTexture.filterMode = FilterMode.Point;
			for(float x = 0; x < 1; x+=1f/SIZE) {
				for(float y = 0; y < 1; y+=1f/SIZE) {
					m_ColorTexture.SetPixel(Mathf.FloorToInt(x * SIZE), Mathf.FloorToInt(y * SIZE), ColorAt(x,y));
				}
			}
			m_ColorTexture.Apply();
		}
		m_ColorImage.texture = m_ColorTexture;
		m_ColorImage.uvRect = new Rect(-m_Padding,-m_Padding,1 + m_Padding * 2,1 + m_Padding * 2);
	}

	override public void SetValue(Vector2 normalizedValue, bool setCursor = true) {
		base.SetValue(normalizedValue, setCursor);
		m_Color = ColorAt(normalizedValue.x, normalizedValue.y);
		if (onColorChanged != null)
			onColorChanged.Invoke(m_Color);
	}

	Color ColorAt(float x, float y) {
		if (m_CustomPalette)
			return m_CustomPalette.GetPixel(Mathf.FloorToInt(x * m_CustomPalette.width), Mathf.FloorToInt(y * m_CustomPalette.height));
		// float a = Mathf.Cos(x);
		// float d = Vector2.Distance(new Vector2(x,y),Vector2.one * .5f);
		// return Color.HSVToRGB(a,d,1.0f);
		x = Mathf.Floor(x * SIZE) / SIZE;
		y = Mathf.Floor(y * SIZE) / SIZE;
		return Color.HSVToRGB(x,y,1.0f);
	}

}
}