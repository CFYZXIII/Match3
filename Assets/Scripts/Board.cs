using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;


public class Board : MonoBehaviour
{
    [SerializeField] private Tile tilePrefab; //префаб фишки
    [SerializeField] Text stepField; //текстовое поле с шагами
    [SerializeField] Button startButton;//кнопка старта игры
    [SerializeField] int width;//ширина доски
    [SerializeField] int height;//высота доски
    [SerializeField] List<Sprite> sprites;//список спрайтов
    private Tile[,] tiles;//массив фишек

    public static bool GameStarted;//флаг начала игры, разрешающий уничтожать фишки
  
    int stepCount; //количество оставшихся шагов
    int nullCount; //количество уничтоженных фишек
  
    List<Tile> AdedTiles = new List<Tile>();//список добавленных фишек, нужен чтобы не создавать фишки заново, при перезапуске игры

    private void Start()
    {
        tiles = new Tile[width, height];
        
    }


    
   /// <summary>
   /// корутина создания/переиспользования созданных фишек
   /// </summary>
   /// <param name="isFirstTime">флаг показывающий, запущена ли игра в первый раз</param>
   /// <returns></returns>
    IEnumerator AddTiletoBoard(bool isFirstTime) 
    {
        GameStarted = false;
        Tile tile = null;
        int s = 0;
        yield return new WaitForSeconds(1);
        for (int j = 0; j < height; j++   )
        {
            for (int i = 0; i < width; i++)
            {
                if (isFirstTime)
                {
                    tile = Instantiate(tilePrefab);
                    tile.transform.parent = this.transform;
                    tile.dropEvent += Tile_dropEvent;
                    AdedTiles.Add(tile);

                }
                else 
                {
                    tile = AdedTiles[s];
                    tile.gameObject.SetActive(true);
                    s++;
                }


                tiles[i, j] = tile;
                
                tile.sprite = sprites[Random.Range(0, 5)];
                                
                tile.x = i;
                tile.y = j;
               
                yield return new WaitForSeconds(0.01f);
                stepCount = 3;
                stepField.text = "STEPS = " + stepCount;
            }
        }
        GameStarted = true;
        nullCount = 0;

    }
    /// <summary>
    /// Метод заполения доски
    /// </summary>
    public void FillBoard() 
    {
        bool isFirstTime = AdedTiles.Count == 0;
        StartCoroutine(AddTiletoBoard(isFirstTime));
    }

    /// <summary>
    /// Метод вызываемый по нажатию на фишку
    /// </summary>
    /// <param name="sender">фишка</param>
    /// <param name="e">событие клика на фишку</param>
    private void Tile_dropEvent(object sender, Event e)
    {
        Tile tile = sender as Tile;

        List<Tile> matchedList = GetMatches(tile, null);
        if (matchedList.Count > 2) 
        {
          
            foreach (Tile t in matchedList)
            {
                t.gameObject.SetActive(false);
                nullCount++;
            }

            stepCount += matchedList.Count - 2;
            stepField.text = "STEPS = " + stepCount;
            DestroyAllMAtches();

        }

        if (matchedList.Count < 3) 
        {
            tile.gameObject.SetActive(false);
            DestroyAllMAtches(tile);
            stepCount -= 1;
            nullCount++;
        }

        if (stepCount > 0)
            stepField.text = "STEPS = " + stepCount;
        else
        {
            stepField.color = Color.red;
            stepField.text = "LOSE =(";
            GameStarted = false;
            startButton.gameObject.SetActive(true);
        }

        if (nullCount == width * height)
        {
            startButton.gameObject.SetActive(true);
            stepField.color = Color.green;
            stepField.text = "WIN!!!";
        }



    }

    /// <summary>
    /// Метод уничтожения фишек
    /// </summary>
    /// <param name="t">фишка, передаётся в метод если найдено меньше 3х совпадений</param>
    void DestroyAllMAtches(Tile t = null) 
    {
        int start, fin;
        if (t == null)
        {
            start = 0;
            fin = width;
        }
        else 
        {
            start = t.x;
            fin = start+1;
        }
        int greenNumber = 0;
        for (int i = start; i < fin; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (tiles[i, j] != null)
                {
                    if (!tiles[i, j].gameObject.activeInHierarchy)
                    {
                        greenNumber++;
                        tiles[i, j] = null;                      
                    }
                    else
                    if (greenNumber > 0)
                    {
                        tiles[i, j].y -= greenNumber;
                        TileMove(tiles[i, j], greenNumber);
                        tiles[i, j - greenNumber] = tiles[i, j];
                        tiles[i, j] = null;
                       
                    }
                }
            }
            greenNumber = 0;
        }
    }
   
    /// <summary>
    /// Метод поиска соседних фишек у фишки на которую нажали
    /// </summary>
    /// <param name="tile">Фишка на которую нажали</param>
    /// <returns></returns>
    private List<Tile> GetNeighbors(Tile tile) 
    {
        Tile leftNeighbor = tile.x > 0 ? tiles[tile.x - 1, tile.y] : null;
        Tile rightNeighbor = tile.x < width - 1 ? tiles[tile.x + 1, tile.y] : null;
        Tile downNeighbor = tile.y > 0 ? tiles[tile.x, tile.y - 1] : null;
        Tile upNeighbor = tile.y < height - 1 ? tiles[tile.x, tile.y + 1] : null;

        List<Tile> neighbors = new List<Tile>();
        neighbors.Add(leftNeighbor);
        neighbors.Add(rightNeighbor);
        neighbors.Add( upNeighbor);
        neighbors.Add(downNeighbor);

        return neighbors;
        
    }
    /// <summary>
    /// Метод поиска совпадений
    /// </summary>
    /// <param name="tile">фишка на которую нажали</param>
    /// <param name="excludedTiles">список исключённых фишек</param>
    /// <returns></returns>
    private List<Tile> GetMatches(Tile tile,List<Tile> excludedTiles = null) 
    {
        if (excludedTiles == null)
        {
            excludedTiles = new List<Tile>();
            excludedTiles.Add(tile);
        }
        else
            excludedTiles.Add(tile);

        List<Tile> matchedTiles = new List<Tile>();
        matchedTiles.Add(tile);

        foreach (Tile neighborTile in GetNeighbors(tile))
        {
            if (neighborTile == null || excludedTiles.Contains(neighborTile) || neighborTile.sprite != tile.sprite) continue;
            matchedTiles.AddRange(GetMatches(neighborTile,excludedTiles));
        }

        return matchedTiles;


    }
    /// <summary>
    /// перемещение верхних фишек вниз, в зависимости от того сколько фишек снизу было уничтожено
    /// </summary>
    /// <param name="tile">фишка</param>
    /// <param name="y">расстояние на которое нужно опуститься</param>
    void TileMove(Tile tile, int y) 
    {
        tile.transform.position -= new Vector3(0, y, 0);
    }

    /// <summary>
    /// При закрытии сцены нужно отписать все фишки от событий
    /// </summary>
    private void OnDestroy()
    {
        foreach(Tile t in AdedTiles)
            t.dropEvent -= Tile_dropEvent;
    }
}
