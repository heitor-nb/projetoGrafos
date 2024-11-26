using ProjetoGrafos;
using System;
using System.Collections.Concurrent;

var endereco = "C:/Users/heito/source/repos/ProjetoGrafosTeste/projetoGrafos/config.txt";
var fileReader = new FileReader(endereco);
var lista = fileReader.GetLinhas(); // pega apenas as linhas de configuração
foreach (var item in lista) Console.WriteLine(item);

try
{
    var n = lista[0][1] - '0'; //var n = (int)char.GetNumericValue(lista[0][1]);
    var grafo = new Graph(n);

    var index = 1;
    while(lista[1][index] != ' ')
    {
        grafo.AddNode(lista[1][index]);
        index++;
    }

    index = 1;
    bool controle = lista[2][index] != '-';
    while (controle)
    {
        var p1 = lista[2][index] - '0';
        var p2 = lista[2][index + 1] - '0';
        var peso = lista[2][index + 2] - '0';
        grafo.AddVizinho(p1, p2, peso); // ponto 1, ponto 2, peso da aresta
        if (lista[2][index + 3] == '-') controle = false;
        index += 4;
    }

    grafo.ExibirPesos();

    grafo.ExibirVizinhos();

    //grafo.ExibirCaminhosMinimos();

    grafo.FloydWarshall();

    //ConcurrentDictionary<int, bool> visitados = new();

    //visitados.TryAdd(0, true);

    //Task dfsTask1 = Task.Run(() => grafo.DFS('a', 0, visitados));
    //Task dfsTask2 = Task.Run(() =>
    //{
    //    Thread.Sleep(100); // representa que o caminhão dois saiu em segundo lugar
    //    grafo.DFS('b', 0, visitados);
    //});

    //await Task.WhenAll(dfsTask1, dfsTask2);
}
catch (Exception ex)
{
    Console.WriteLine($"Erro: {ex.Message}");
}


//var grafo = new Graph(8);
//grafo.AddNode('0');
//grafo.AddNode('1');
//grafo.AddNode('2');
//grafo.AddNode('3');
//grafo.AddNode('4');
//grafo.AddNode('5');
//grafo.AddNode('6');
//grafo.AddNode('7');

//grafo.AddVizinho(0, 1, 5);
//grafo.AddVizinho(0, 2, 4);
//grafo.AddVizinho(1, 3, 2);
//grafo.AddVizinho(2, 3, 3);
//grafo.AddVizinho(2, 5, 1);
//grafo.AddVizinho(3, 4, 1);
//grafo.AddVizinho(3, 6, 4);
//grafo.AddVizinho(3, 7, 7);
//grafo.AddVizinho(6, 7, 2);

//grafo.ExibirPesos();

//ConcurrentDictionary<int, bool> visitados = new();

//Task bfsTask1 = Task.Run(() => grafo.BFS(0, visitados));
//Task bfsTask2 = Task.Run(() => grafo.BFS(6, visitados));

//await Task.WhenAll(bfsTask1, bfsTask2);
