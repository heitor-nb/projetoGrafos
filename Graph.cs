﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace ProjetoGrafos
{
    internal class Graph
    {
        private readonly int maxSize;
        private int nodeCount;
        private readonly int lixoPorLata = 5; // m^3 de lixo por lata **
        public Node[] Vetor { get; set; }
        public int[,] W { get; set; }

        public Graph(int n)
        {
            Vetor = new Node[n];
            maxSize = n;
            nodeCount = 0;
            W = new int[n, n];
            for(int i = 0; i < n; i++)
            {
                for(int j = 0; j < n; j++)
                {
                    W[i, j] = -1;
                }
            }
        }

        public void AddNode(char c)
        {
            
            if(nodeCount < maxSize)
            {
                var node = new Node(c)
                {
                    Index = nodeCount
                };
                Vetor[nodeCount] = node;
                //for(int i = 0; i < Vetor.Length; i++)
                //{
                //    W[nodeCount, i] = -1;
                //    W[i, nodeCount] = -1;
                //}
                W[nodeCount, nodeCount] = 0;
                nodeCount++;
                Console.WriteLine("Adicionado.");
            }
            else Console.WriteLine("Vetor cheio.");
        }

        public void AddVizinho(int i, int j, int peso)
        {
            if(i <= nodeCount && j <= nodeCount)
            {
                Vetor[i].N.Add(Vetor[j]); // *
                Vetor[j].N.Add(Vetor[i]);
                W[i, j] = peso;
                W[j, i] = peso;
                Console.WriteLine("Vizinho adicionado.");
            }
            else Console.WriteLine("Algum ou ambos vértices não existem.");
        }

        public void ExibirPesos()
        {
            for(int i = 0; i < maxSize; i++)
            {
                for(int  j = 0; j < maxSize; j++)
                {
                    if(W[i, j] == -1) Console.Write("-   ");
                    else Console.Write($"{W[i, j]}   ");
                }
                Console.WriteLine();
            }
        }

        public void ExibirVizinhos()
        {
            foreach(var node in Vetor)
            {
                Console.Write($"{node.Symbol}: ");
                foreach (var v in node.N) Console.Write($"{v.Symbol}, ");
                Console.WriteLine();
            }
        }

        //public void BFS()
        //{
        //    if(nodeCount > 0)
        //    {
        //        var marked = new bool[nodeCount];
        //        for (int i = 0; i < nodeCount; i++) marked[i] = false;
        //        var fila = new Queue<Node>();
        //        fila.Enqueue(Vetor[0]);
        //        marked[Vetor[0].Index] = true;
        //        Console.Write($"{Vetor[0].Symbol}");
        //        while (fila.Count > 0)
        //        {
        //            var aux = fila.Dequeue();
        //            foreach(var n in aux.N)
        //            {
        //                if (!marked[n.Index])
        //                {
        //                    fila.Enqueue(n);
        //                    marked[n.Index] = true;
        //                    Console.Write($" -> {n.Symbol}");
        //                }
        //            }
        //        }
        //    }
        //}

        // Estudar melhor os métodos relacionados ao ConcurrentDictionary.
        // fazer correções relacionadas ao tempo que já constam no DFS **
        public void BFS(int v, ConcurrentDictionary<int, bool> visitados)
        {
            if (v < nodeCount && nodeCount > 0)
            {
                //var marked = new bool[nodeCount];
                //for (int i = 0; i < nodeCount; i++) marked[i] = false;
                var fila = new Queue<Node>();
                fila.Enqueue(Vetor[v]);
                //marked[Vetor[v].Index] = true;
                visitados.TryAdd(v, true);
                Console.Write($"{Vetor[v].Symbol}");
                while (fila.Count > 0)
                {
                    var aux = fila.Dequeue();
                    foreach (var n in aux.N)
                    {
                        if (visitados.TryAdd(n.Index, true))
                        {
                            Thread.Sleep(W[aux.Index, n.Index] * 1000);
                            fila.Enqueue(n);
                            Console.Write($" -> {Vetor[v].Symbol} - {n.Symbol}");
                        }
                    }
                }
            }
        }

        public void DFS(char symbol, int v, ConcurrentDictionary<int, bool> visitados)
        {
            if (v < nodeCount && nodeCount > 0)
            {
                //visitados.TryAdd(v, true);
                Thread.Sleep(Vetor[v].LatasLixo * lixoPorLata * 100);
                Console.WriteLine($"{Vetor[v].Symbol}");
                Visitar(symbol, Vetor[v], visitados);
            }
        }

        private void Visitar(char origem, Node v, ConcurrentDictionary<int, bool> visitados)
        {
            Thread.Sleep(v.LatasLixo * lixoPorLata * 100); // tempo para recolher o lixo
            foreach (var n in v.N)
            {
                var estado = visitados.TryAdd(n.Index, true);
                //Console.WriteLine($"Estado ({v.Index} - {n.Index}): {estado}");
                if (estado)
                {
                    Thread.Sleep(W[v.Index, n.Index] * 100); // tempo para chegar ao ponto
                    Console.WriteLine($"({origem})   {v.Symbol} - {n.Symbol}");
                    Visitar(origem, n, visitados);
                    Console.WriteLine($"({origem}) R {n.Symbol} - {v.Symbol}");
                    Thread.Sleep(W[n.Index, v.Index] * 100); // tempo para retornar ao ponto de origem
                }
            }
        }

        // prox passo retornar ao aterro quando finalizar

        public List<Node>[,] FloydWarshall()
        {
            var dist = new int[maxSize, maxSize];
            var prev = new Node?[maxSize, maxSize];

            for (int i = 0; i < maxSize; i++)
            {
                for (int j = 0; j < maxSize; j++)
                {
                    if (W[i, j] != -1)
                    {
                        dist[i, j] = W[i, j];
                        prev[i, j] = Vetor[i];
                    }
                    else
                    {
                        dist[i, j] = 1024;
                        prev[i, j] = null;
                    }
                }
            }

            for (int k = 0; k < maxSize; k++)
            {
                for (int i = 0; i < maxSize; i++)
                {
                    for (int j = 0; j < maxSize; j++)
                    {
                        if (dist[i, j] > dist[i, k] + dist[k, j])
                        {
                            dist[i, j] = dist[i, k] + dist[k, j];
                            prev[i, j] = prev[k, j];
                        }
                    }
                }
            }

            for (int i = 0; i < maxSize; i++)
            {
                for (int j = 0; j < maxSize; j++)
                {
                    Console.WriteLine($"{i} -> {j}: {dist[i, j]}");
                }
            }

            // monta matriz de caminhos:
            var paths = new List<Node>[maxSize, maxSize];
            for(int u = 0; u < maxSize; u++)
            {
                for(int v = 0; v < maxSize; v++)
                {
                    paths[u, v] = Path(prev, u, v);
                }
            }
            return paths;
        }

        private List<Node> Path(Node?[,] prev, int u, int v)
        {
            var path = new List<Node>();
            if (prev[u, v] == null) return path;
            else
            {
                var aux = Vetor[v];
                path.Add(aux);
                while (Vetor[u] != aux)
                {
                    aux = prev[u, v];
                    if(aux != null) path.Add(aux);
                }
                return path;
            }
        }

        //public void ExibirCaminhosMinimos()
        //{
        //    var paths = FloydWarshall();
        //    for (int i = 0; i < maxSize; i++)
        //    {
        //        for (int j = 0; j < maxSize; j++)
        //        {
        //            Console.Write($"{i} -> {j}: ");
        //            foreach (var node in paths[i, j]) Console.Write($"{node.Index} ");
        //            Console.WriteLine();
        //        }
        //    }
        //}
    }
}
