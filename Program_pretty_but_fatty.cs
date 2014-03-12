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
        private static Node _fakeNode = new Node(char.MinValue);
 
        private readonly Node _root;
 
        public const int AlphabetSize = 26;
        public const char FirstChar = 'a';
 
        private readonly int _patternsCount;
 
        public AhoTrie(int patternsCount)
        {
            _patternsCount = patternsCount;
            _root = new Node(char.MaxValue);
        }
 
        private class Node
        {
            public readonly Node[] Children = new Node[AlphabetSize];
            public readonly Node[] Passes = new Node[AlphabetSize];
            public Node Parent;
            public Node SuffixLink;
            public Node ShortenedLink;
            public readonly char Character;
            public bool IsLeaf;
            public List<int> Patterns;
 
            public Node(char c)
            {
                Character = c;
            }
 
            public override string ToString()
            {
                return (char)(Character+FirstChar) + " (" + (Patterns != null ? Patterns.Count : 0) + ")";
            }
        }
 
        Node GetSuffixLink(Node n)
        {
            if (n.SuffixLink == null)
                if (n == _root || n.Parent == _root)
                    n.SuffixLink = _root;
                else
                    n.SuffixLink = GetPass(GetSuffixLink(n.Parent), n.Character);
            return n.SuffixLink;
        }
 
        Node GetPass(Node n, char c)
        {
            if (n.Passes[c] == null)
                if (n.Children[c] != null)
                    n.Passes[c] = n.Children[c];
                else
                    n.Passes[c] = n == _root ? n : GetPass(GetSuffixLink(n), c);
            return n.Passes[c];
        }
 
        Node GetShortenedLink(Node n)
        {
            if (n.ShortenedLink == _fakeNode)
                return null;
            if (n.ShortenedLink == null)
                if (GetSuffixLink(n).IsLeaf)
                    n.ShortenedLink = GetSuffixLink(n);
                else if (GetSuffixLink(n) == _root)
                    n.ShortenedLink = null;
                else
                    n.ShortenedLink = GetShortenedLink(GetSuffixLink(n));
            return n.ShortenedLink;
        }
 
        public void AddString(string s, int patternNumber)
        {
            Node n = _root;
            foreach (var letter in s)
            {
                var c = (char)(letter - FirstChar);
                Node x = n.Children[c];
                if (x == null)
                {
                    x = (n.Children[c] = new Node(c));
                    x.Parent = n;
                    x.IsLeaf = false;
                }
                n = x;
            }
            n.IsLeaf = true;
            if (n.Patterns == null)
                n.Patterns = new List<int>();
            n.Patterns.Add(patternNumber);
        }
 
        private void Check(Node n, bool[] matches)
        {
            while (n != _root)
            {
                if (n.Patterns != null)
                    foreach (var pattern in n.Patterns)
                        matches[pattern] = true;
                n.IsLeaf = false;
                Node nn = GetShortenedLink(n);
                n.ShortenedLink = _fakeNode;
                if (nn == null)
                    return;
                n = nn;   
            }
        }
 
        public bool[] ProcessText(string text)
        {
            var result = new bool[_patternsCount];
            Node n = _root;
            foreach (char t in text)
            {
                var c = (char) (t - FirstChar);
                n = GetPass(n, c);
                Check(n, result);
            }
            return result;
        }
    }
}