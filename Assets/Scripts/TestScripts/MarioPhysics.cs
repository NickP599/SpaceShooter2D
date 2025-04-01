using UnityEngine;

public class MarioPhysics : MonoBehaviour
{
    // Текущая позиция Марио
    private Vector2 position;
    // Текущая скорость Марио
    private Vector2 velocity;
    // Ускорение (в нашем случае гравитация)
    private Vector2 acceleration = new Vector2(0, -1f);

    // Начальные значения
    void Start()
    {
        position = Vector2.zero;    // (0, 0)
        velocity = new Vector2(1, 3); // начальная скорость (1, 3)
    }

    // Обновление на каждом кадре
    void Update()
    {
        // Шаг 1: Добавляем скорость к позиции
        position += velocity * Time.deltaTime;

        // Шаг 2: Добавляем ускорение к скорости
        velocity += acceleration * Time.deltaTime;

        // Применяем вычисленную позицию к объекту в Unity
        transform.position = new Vector3(position.x, position.y, 0);

        PlayerInput();

        // Выводим информацию для отладки
        Debug.Log($"Frame: Position = ({position.x:F2}, {position.y:F2}), " +
                 $"Velocity = ({velocity.x:F2}, {velocity.y:F2})");
    }

    // Метод для имитации ввода игрока
    void PlayerInput()
    {
        // Пример: если игрок нажимает "прыжок"
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Добавляем вертикальную скорость для прыжка
            velocity.y = 5f;
        }
    }
}