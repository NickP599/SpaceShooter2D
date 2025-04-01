using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VirtualJoyStick : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Image m_JoyBack; // Фон джойстика
    [SerializeField] private Image m_JoyStick; // Сам джойстик

    public Vector2 Value { get; private set; } // Текущее значение джойстика

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 position = Vector2.zero;

        // Преобразование экранной точки в локальную точку внутри прямоугольника фона джойстика
        RectTransformUtility.ScreenPointToLocalPointInRectangle(m_JoyBack.rectTransform, eventData.position, eventData.pressEventCamera,
            out position);

        // Нормализация позиции относительно размеров фона джойстика
        position.x = (position.x / m_JoyBack.rectTransform.sizeDelta.x);
        position.y = (position.y / m_JoyBack.rectTransform.sizeDelta.y);

        // Преобразование координат в диапазон от -1 до 1
        position.x = position.x * 2 - 1;
        position.y = position.y * 2 - 1;

        // Установка значения джойстика
        Value = new Vector2(position.x, position.y);

        // Ограничение значения джойстика единичной окружностью
        if (Value.magnitude > 1)
            Value = Value.normalized;

        // Вычисление смещения джойстика относительно фона
        float offSetX = m_JoyBack.rectTransform.sizeDelta.x / 2 - m_JoyStick.rectTransform.sizeDelta.x / 2;
        float offSetY = m_JoyBack.rectTransform.sizeDelta.y / 2 - m_JoyStick.rectTransform.sizeDelta.y / 2;

        // Установка позиции джойстика
        m_JoyStick.rectTransform.anchoredPosition = new Vector2(Value.x * offSetX, Value.y * offSetY);

        //Debug.Log(Value); // Вывод значения джойстика в консоль
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData); // Обработка события нажатия
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Value = Vector2.zero; // Сброс значения джойстика
        m_JoyStick.rectTransform.anchoredPosition = Vector2.zero; // Сброс позиции джойстика
    }
}
 