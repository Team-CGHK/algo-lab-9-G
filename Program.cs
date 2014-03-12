using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace AlgorighmsLab9_G
{
    class Program
    {
        static void Main(string[] args)
        {
            StreamReader sr = new StreamReader("search4.in");
            StreamWriter sw = new StreamWriter("search4.out");
            int n = int.Parse(sr.ReadLine());
            AhoTrie tree = new AhoTrie(n);
            for (int i = 0; i < n; i++)
                tree.AddString(sr.ReadLine(), i);
            var answer = tree.ProcessText(sr.ReadLine());
            foreach (var b in answer)
                sw.WriteLine(b ? "YES" : "NO");
            sw.Close();
        }
    }

    class AhoTrie
    {
        private List<Node> nodes = new List<Node>();
        private int AddNode(Node n) 
        {
            nodes.Add(n);
            return nodes.Count - 1;
        }

        private Node GetNode(int x)
        {
            if (x < 0 || x >= nodes.Count) return null;
            return nodes[x];
        }

        private const int FakeNode = -2;
        private readonly int _root;

        public const int AlphabetSize = 26;
        public const char FirstChar = 'a';

        private readonly int _patternsCount;

        public AhoTrie(int patternsCount)
        {
            _patternsCount = patternsCount;
            _root = AddNode(new Node(char.MaxValue));
        }

        private class Node
        {
            public int[] Children;
            public int[] Passes;
            public int Parent = -1;
            public int SuffixLink = -1;
            public int ShortenedLink = -1;
            public readonly char Character;
            public bool IsLeaf;
            public List<int> Patterns;

            public Node(char c)
            {
                Character = c;
            }

            public override string ToString()
            {
                return (char)(Character + FirstChar) + " (" + (Patterns != null ? Patterns.Count : 0) + ")";
            }
        }

        int GetSuffixLink(int n)
        {
            Node x = GetNode(n);
            if (x.SuffixLink == -1)
                if (n == _root || x.Parent == _root)
                    x.SuffixLink = _root;
                else
                    x.SuffixLink = GetPass(GetSuffixLink(x.Parent), x.Character);
            return x.SuffixLink;
        }

        int[] MakeEmptyArray(int size)
        {
            int[] result = new int[size];
            for (int i = 0; i < size; ++i)
                result[i] = -1;
            return result;
        }

        int GetPass(int n, char c)
        {
            Node x = GetNode(n);
            if (x.Passes == null)
                x.Passes = MakeEmptyArray(AlphabetSize);
            if (x.Passes[c] == -1)
            {
                if (x.Children != null && GetNode(x.Children[c]) != null)
                    x.Passes[c] = x.Children[c];
                else
                    x.Passes[c] = n == _root ? n : GetPass(GetSuffixLink(n), c);
            }
            return x.Passes[c];
        }

        int GetShortenedLink(int n)
        {
            Node x = GetNode(n);
            if (x.ShortenedLink == FakeNode)
                return FakeNode;
            if (x.ShortenedLink == -1)
                if (GetNode(GetSuffixLink(n)).IsLeaf)
                    x.ShortenedLink = GetSuffixLink(n);
                else if (GetSuffixLink(n) == _root)
                    x.ShortenedLink = -1;
                else
                    x.ShortenedLink = GetShortenedLink(GetSuffixLink(n));
            return x.ShortenedLink;
        }

        public void AddString(string s, int patternNumber)
        {
            int n = _root;
            foreach (var letter in s)
            {
                Node m = GetNode(n);
                var c = (char)(letter - FirstChar);
                if (m.Children == null)
                    m.Children = MakeEmptyArray(AlphabetSize);
                int x = m.Children[c];
                if (x == -1)
                {
                    x = (m.Children[c] = AddNode(new Node(c)));
                    GetNode(x).Parent = n;
                    GetNode(x).IsLeaf = false;
                }
                n = x;
            }
            Node a = GetNode(n);
            a.IsLeaf = true;
            if (a.Patterns == null)
                a.Patterns = new List<int>();
            a.Patterns.Add(patternNumber);
        }

        private void Check(int n, bool[] matches)
        {
            while (n != _root && n != FakeNode)
            {
                Node x = GetNode(n);
                if (x.Patterns != null)
                    foreach (var pattern in x.Patterns)
                        matches[pattern] = true;
                x.IsLeaf = false;
                x.Patterns = null;
                int nn = GetShortenedLink(n);
                x.ShortenedLink = FakeNode;
                if (nn == -1)
                    return;
                n = nn;
            }
        }

        public bool[] ProcessText(string text)
        {
            var result = new bool[_patternsCount];
            int n = _root;
            foreach (char t in text)
            {
                var c = (char)(t - FirstChar);
                n = GetPass(n, c);
                Check(n, result);
            }
            return result;
        }
    }
}