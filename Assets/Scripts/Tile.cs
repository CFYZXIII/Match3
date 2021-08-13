using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public int x;//горизонтальная координата фишки
    public int y;//вертикальная координата фишки
    public Sprite sprite;//спрайт фишки

    /// <summary>
    /// событие вызываемое при нажатии на фишку
    /// </summary>
    public event Ontriggered dropEvent;

    /// <summary>
    /// делегат метода вызываемого при нажатии на фишку
    /// </summary>
    /// <param name="sender">фишка</param>
    /// <param name="e">собие</param>
    public delegate void Ontriggered(object sender, Event e);

    /// <summary>
    /// Метож выхываемый по нажатии на фишку
    /// </summary>
    private void OnMouseDown()
    {
        if(Board.GameStarted)
        dropEvent?.Invoke(this, new Event());
    }

    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = sprite;
        transform.localPosition = new Vector2(x, y);
    }


}
