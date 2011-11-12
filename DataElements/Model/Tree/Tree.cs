using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace DataElements.Model.Tree
{

    public interface I_Tree
    {
        List<int> GetInOrder();
    }

    public class MyTree : I_Tree
    {
        Node root;
        public int depth = 0;
        int tempDepth = 0;


        public void AddValue(int n)
        {
            tempDepth = 0;

            if (root != null)
            {
               AddValue(n, root);
               depth = tempDepth + 1;
            }
            else
            {
                root = new Node();
                root.value = n;
                depth = 1;
            }
        }

        private void AddValue(int value, Node n)
        {
            bool insertLeft = (n.value == value) || (n.value >= value && n.left == null);

            if (insertLeft)
            {
                Node newNode = new Node();
                newNode.value = value;
                newNode.left = n.left;
                newNode.right = null;
                n.left = newNode;
                tempDepth++;
                return;
            }

            bool insertRight = (n.value < value && n.right == null);

            if (insertRight)
            {
                Node newNode = new Node();
                newNode.value = value;
                newNode.right = n.right;
                newNode.left = null;
                n.right = newNode;
                tempDepth++;
                return;
            }

            if (!insertLeft && !insertRight)
            {
                if (value <= n.value)
                {
                    tempDepth++;
                    AddValue(value, n.left);
                }
                else
                {
                    AddValue(value, n.right);
                    tempDepth++;
                }
            }
        }

        public List<int> GetInOrder()
        {
            return GetInOrder(root, new List<int>());
        }

        private List<int> GetInOrder(Node node, List<int> workingList)
        {
            if (node.left != null)
                GetInOrder(node.left, workingList);
            workingList.Add(node.value);
            if (node.right != null)
                GetInOrder(node.right, workingList);

            return workingList;
        }

    }

    public class Nodes : List<Node>
    {
    }

    public class Node
    {
        public int value;
        public Node left;
        public Node right;
    }


 

}
