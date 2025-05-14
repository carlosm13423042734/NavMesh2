using UnityEngine;

using System.Collections.Generic;
using Palmmedia.ReportGenerator.Core.Reporting.Builders;


public class MazeGenerator : MonoBehaviour
{
    public GameObject wallPrefab; // Prefab de las paredes
    public float wallHeight;
    public float wallLength;
    public int mazeWidth;//Ancho del laberinto
    public int mazeHeight;    //largodel laberinto
    public Vector2Int startPosition;//Posici�n de entrada --- OJETE estos valores cambiarlos                     
    public Vector2Int endPosition;//Posici�n de salida --- OJETE estos valores cambiarlos                     

    //Matriz que celdas vistadas
    private bool[,] visited;

    //Matriz tridimiensional para almacenar las referencias de las paredes
    /*
     * walls[x,z,0]= Pared del norte
     * walls[x,z,1]= Pared del este 
     * walls[x,z,2]= Pared del sur
     * walls[x,z,3]= Pared del oeste
     * 
     */
    private GameObject[,,] walls;

    //Vector de direcciones
    /*
     * Apuntamos en los 4 putnos cardinales
     * Norte (0,1)
     * Sur (0,-1)
     * Este (1,0)
     * Oeste (-1,0)
     * Este vector es el que nos va a ayudar a movernos entre celdas
     */
    private Vector2Int[] directions = new Vector2Int[4]
    {
        new Vector2Int(0,1), //Norte
        new Vector2Int(1,0), //Este
        new Vector2Int(0,-1), //Sur
        new Vector2Int(-1,0), //Oeste
    };


    /*********
     * Start
     * 
     * 
    */
    // Start is called before the first frame update
    void Start()
    {
        // Paso 1. Inicializar las estructuras de datos y creamos todas las pardes
        Init();
        // Paso 2. 
        GenerateMaze();
        // Paso 3. Abrir los puntos de entrada y salida
        ClearIO();
        // Paso 4 Marcamos la entrada y la salida
        Mark();
    }//Start

    private void Mark()
    {

        // Crear un material verde para la entrada
        Material entryMaterial = new Material(Shader.Find("Standard"));
        entryMaterial.color = Color.green;

        // Crear un material rojo para la salida
        Material exitMaterial = new Material(Shader.Find("Standard"));
        exitMaterial.color = Color.red;

        // Crear un cubo plano verde para marcar la entrada
        GameObject entry = GameObject.CreatePrimitive(PrimitiveType.Cube);
        entry.transform.position = new Vector3(startPosition.x * wallLength + wallLength / 2, 0.1f, startPosition.y * wallLength + wallLength / 2);
        entry.transform.localScale = new Vector3(wallLength * 0.5f, 0.1f, wallLength * 0.5f);
        entry.GetComponent<Renderer>().material = entryMaterial;
        entry.transform.parent = transform;
        entry.name = "Entry";

        // Crear un cubo plano rojo para marcar la salida
        GameObject exit = GameObject.CreatePrimitive(PrimitiveType.Cube);
        exit.transform.position = new Vector3(endPosition.x * wallLength + wallLength / 2, 0.1f, endPosition.y * wallLength + wallLength / 2);
        exit.transform.localScale = new Vector3(wallLength * 0.5f, 0.1f, wallLength * 0.5f);
        exit.GetComponent<Renderer>().material = exitMaterial;
        exit.transform.parent = transform;
        exit.name = "Exit";


    }
    private void Init()
    {
        //Incializamos las celdas visitadas
        visited = new bool[mazeWidth, mazeHeight];

        walls = new GameObject[mazeWidth, mazeHeight, 4];

        //Creamos un objeto padre. Para tener una referencia con el padre
        GameObject wallsParent = new GameObject("Walls");
        wallsParent.transform.parent = this.transform;//Cojo la referncia

        //creamos las 4 paredes para cada celda del laberintico
        for (int x = 0; x < mazeWidth; x++)
        {
            for (int z = 0; z < mazeHeight; z++)
            {
                //Creamos la pared norte
                walls[x, z, 0] = CreateWall(x, z, 0, wallsParent.transform);
                //Creamos la pared este
                walls[x, z, 1] = CreateWall(x, z, 1, wallsParent.transform);
                //Creamos la pared sur
                walls[x, z, 2] = CreateWall(x, z, 2, wallsParent.transform);
                //Creamos la pared oeste
                walls[x, z, 3] = CreateWall(x, z, 3, wallsParent.transform);

            }//for z

        }// for x

    }
    private void ClearIO()
    {
        // Determinar qu� pared eliminar para la entrada seg�n su posici�n en el laberinto
        if (startPosition.x == 0) // Entrada en el borde izquierdo
        {
            RemoveWall(startPosition.x, startPosition.y, 3); // Eliminar pared oeste
        }
        else if (startPosition.x == mazeWidth - 1) // Entrada en el borde derecho
        {
            RemoveWall(startPosition.x, startPosition.y, 1); // Eliminar pared este
        }
        else if (startPosition.y == 0) // Entrada en el borde inferior
        {
            RemoveWall(startPosition.x, startPosition.y, 2); // Eliminar pared sur
        }
        else if (startPosition.y == mazeHeight - 1) // Entrada en el borde superior
        {
            RemoveWall(startPosition.x, startPosition.y, 0); // Eliminar pared norte
        }

        // Determinar qu� pared eliminar para la salida seg�n su posici�n en el laberinto
        if (endPosition.x == 0) // Salida en el borde izquierdo
        {
            RemoveWall(endPosition.x, endPosition.y, 3); // Eliminar pared oeste
        }
        else if (endPosition.x == mazeWidth - 1) // Salida en el borde derecho
        {
            RemoveWall(endPosition.x, endPosition.y, 1); // Eliminar pared este
        }
        else if (endPosition.y == 0) // Salida en el borde inferior
        {
            RemoveWall(endPosition.x, endPosition.y, 2); // Eliminar pared sur
        }
        else if (endPosition.y == mazeHeight - 1) // Salida en el borde superior
        {
            RemoveWall(endPosition.x, endPosition.y, 0); // Eliminar pared norte
        }

    }//ClearIO

    private void GenerateMaze()
    {
        //Cogemos la posici�n de entrada
        int startX = startPosition.x;
        int startZ = startPosition.y;
        /// Utilizamos un algoritmo recursivo llamado Backtraking. 
        genFrom(startX, startZ);

    }//GenerateMaze


    /// <summary>
    /// Algoritmo recursivo. 
    /// Para cada celda, se va a elegir de manera aleatoria una direcci�n vecina que hayamos visitado.
    /// Entre estas dos celdas hay una pared que se eliminar� y contin�a el proceso desde la nueva celda.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    private void genFrom(int x, int z)
    {
        //1� Marcamos como visitada
        visited[x, z] = true;

        //Creo una lista con las cuatro direcciones y luego las barajo
        List<int> dirs = new List<int> { 0, 1, 2, 3 };
        Shuffle(dirs);

        //Para cada direcci�n 
        foreach (int dir in dirs)
        {
            //calculamos las nuevas coordenadas del vecino
            int nx = x + directions[dir].x;
            int nz = z + directions[dir].y;

            //Verificamos que estamos en los l�mites y que no la hemos visitado
            if (nx >= 0 && nx < mazeWidth && nz >= 0 && nz < mazeHeight && !visited[nx, nz])
            {
                RemoveWall(x, z, dir);
                genFrom(nx, nz);

            }//if


        }//foreach
    }//genFrom
    private void RemoveWall(int x, int z, int dir)
    {
        //destruimos pared en la direcci�n que nos dice, si existe
        if (walls[x, z, dir] != null)
        {
            Destroy(walls[x, z, dir]);
            walls[x, z, dir] = null;
        }//if

        //Calculamos las coordenads de la celda vecina
        int nx = x + directions[dir].x;
        int nz = z + directions[dir].y;

        int posOpposite = (dir + 2) % 4;//Cogemos la celda opuesta

        if (nx >= 0 && nx < mazeWidth && nz >= 0 && nz < mazeHeight && walls[nx, nz, posOpposite] != null)
        {
            Destroy(walls[nx, nz, posOpposite]);
            walls[nx, nz, posOpposite] = null;
        }//if


    }//RemoveWall

    private void Shuffle(List<int> dirs)
    {
        for (int i = 0; i < dirs.Count; i++)
        {
            int temp = dirs[i];
            int random = Random.Range(i, dirs.Count);
            dirs[i] = dirs[random];
            dirs[random] = temp;
        }//for

    }//Shuffle


    /// <summary>
    /// M�todo para inicializar todos las estructuras
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    //Init
    /// <summary>
    /// M�todo para crear una pared individual
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <param name="direction">0 Norte,1 este, 2 sur, 3 oeste</param>
    /// <param name="transform">El objeto padre al que va a asignar la pared</param>
    /// <returns></returns>
    private GameObject CreateWall(int x, int z, int direction, Transform transform)
    {
        //Instanciamos el prefab 
        GameObject wall = Instantiate(wallPrefab);
        wall.transform.parent = transform;

        // posicionamos 
        switch (direction)
        {
            case 0:
                // Norte. Borde superior
                wall.transform.position = new Vector3(x * wallLength + wallLength / 2, wallHeight / 2, z * wallLength + wallLength);
                // escalar para que ocupe toda la celda
                wall.transform.localScale = new Vector3(wallLength, wallHeight, 0.1f);
                break;

            case 1: //Este
                wall.transform.position = new Vector3(x * wallLength + wallLength, wallHeight / 2, z * wallLength + wallLength / 2);
                // escalar para que ocupe toda la celda
                wall.transform.localScale = new Vector3(0.1f, wallHeight, wallLength);
                break;
            case 2://Sur
                wall.transform.position = new Vector3(x * wallLength + wallLength / 2, wallHeight / 2, z * wallLength);
                // escalar para que ocupe toda la celda
                wall.transform.localScale = new Vector3(wallLength, wallHeight, 0.1f);
                break;

            case 3: //Oeste
                wall.transform.position = new Vector3(x * wallLength, wallHeight / 2, z * wallLength + wallLength / 2);
                // escalar para que ocupe toda la celda
                wall.transform.localScale = new Vector3(0.1f, wallHeight, wallLength);
                break;
        }//switch
        return wall;

    }//CreateWall





}//MazeGanerator
