using UnityEngine;
using System.Text;
using System.Collections.Generic;
using System.Collections;

public class LSystem : MonoBehaviour
{
    public int iterations;
    public float angle;
    public float width;
    public float minLeafLength;
    public float maxLeafLength;
    public float minBranchLength;
    public float maxBranchLength;
    public float variance;

    public GameObject tree;
    public GameObject branch;
    public GameObject leaf;

    public string FRule;
    public string XRule;

    private const string axiom = "X";

    private Dictionary<char, string> rules = new Dictionary<char, string>();
    private Stack<SavedTransform> savedTransforms = new Stack<SavedTransform>();
    private Vector3 initialPosition;

    private string currentPath = "";
    private float[] randomRotations;

    [ContextMenu("Generate")]
    private void Awake()
    {
        randomRotations = new float[1000];
        for (int i = 0; i < randomRotations.Length; i++)
        {
            randomRotations[i] = Random.Range(-1f, 1f);
        }

        rules.Add('X', XRule);
        rules.Add('F', FRule);

        Generate();
    }

    
    private void Generate()
    {
        currentPath = axiom;

        StringBuilder stringBuilder = new StringBuilder();

        for (int i = 0; i < iterations; i++)
        {
            char[] currentPathChars = currentPath.ToCharArray();
            for (int j = 0; j < currentPathChars.Length; j++)
            {
                stringBuilder.Append(
                    rules.ContainsKey(currentPathChars[j]) ?
                    rules[currentPathChars[j]] :
                    currentPathChars[j].ToString());
            }

            currentPath = stringBuilder.ToString();
            stringBuilder = new StringBuilder();
        }
        for (int k = 0; k < currentPath.Length; k++)
        {
            
            switch (currentPath[k])
            {
                case 'F':

                    GameObject newBranch;
                    bool isLeaf = false;

                    if (currentPath[k+1] % currentPath.Length == 'X'
                        || currentPath[k + 3] % currentPath.Length == 'F'
                        && currentPath[k+4] % currentPath.Length == 'X')
                    {
                        isLeaf = true;
                        newBranch = Instantiate(leaf, transform.position, transform.rotation); 
                    }  else
                    {
                        newBranch = Instantiate(branch, transform.position, transform.rotation);
                    }

                    newBranch.transform.SetParent(tree.transform);
                    var lr = newBranch.GetComponent<LineRenderer>();

                    lr.startWidth *= width;
                    lr.endWidth *= width;

                    // lr.SetPosition(0, initialPosition);

                    float length;

                    if (isLeaf)
                    {
                        length = Random.Range(minLeafLength, maxLeafLength);
                    } else
                    {
                        length = Random.Range(minBranchLength, maxBranchLength);
                    }

                    transform.Translate(Vector3.up * length);

                    lr.SetPosition(1, new Vector3(0, length, 0));
                    
                    break;

                case 'X':
                    break;
                case 'W':
                    width *= 1.1f;
                    break;
                case 'w':
                    width *= 0.9f;
                    break;
                case '+':
                    transform.Rotate(Vector3.forward * angle * (1f + variance / 100f * randomRotations[k % randomRotations.Length]));
                    break;
                case '-':
                    transform.Rotate(Vector3.back * angle * (1f + variance / 100f * randomRotations[k % randomRotations.Length]));
                    break;
                case '*':
                    transform.Rotate(Vector3.up * angle * (1f + variance / 100f * randomRotations[k % randomRotations.Length]));
                    break;
                case '/':
                    transform.Rotate(Vector3.down * angle * (1f + variance / 100f * randomRotations[k % randomRotations.Length]));
                    break;
                case '[':
                    savedTransforms.Push(new SavedTransform()
                    {
                        position = transform.position,
                        rotation = transform.rotation
                    });
                    break;
                case ']':
                    SavedTransform savedTransform = savedTransforms.Pop();
                    transform.position = savedTransform.position;
                    transform.rotation = savedTransform.rotation;
                    break;
            }


        }
         
    }
}