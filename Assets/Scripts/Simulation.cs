using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Simulation : MonoBehaviour
{

    private ResearchSimulation currentSim;
    private string currentTest;
    private ResearchTest currentResearchTest;

    public GameObject petriDish;
    public GameObject controlPetriDish;
    public GameObject bacteriaPrefab;

    public Text startText;
    public Text endText;
    public Text timeText;
    public Text amountText;
    public Text percentText;

    public Slider timeSlider;
    public Button playButton;

    public Graph graph;
    public Graph controlGraph;

    private List<GameObject> bacteriaImages = new List<GameObject>();
    private List<GameObject> controlImages = new List<GameObject>();

    public float currentTime;
    public int maximumBacteria = 30;
    public int startingBacteria = 15;
    public float guessAmount = 10;

    private float startTime;
    private float endTime;
    private float controlAmt;

    private bool isPaused = true;

    private List<double> bestFitCoeffs;
    private List<double> controlFitCoeffs;

    // Start is called before the first frame update
    void Start()
    {
        GenerateBacteriaImages();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateGuessAmount(string amt)
    {
        float num;
        if (float.TryParse(amt, out num))
        {
            if (num >= 0)
            {
                StopAllCoroutines();
                guessAmount = num;
                SetUpSimulation(currentSim, currentTest);
            }
        }
        
    }

    public void SetUpSimulation(ResearchSimulation simulation, string test)
    {
        StopAllCoroutines();
        currentSim = simulation;
        currentTest = test;
        currentResearchTest = currentSim.GetTest(currentTest);

        bestFitCoeffs = FindPolynomialLeastSquaresFit(currentResearchTest.GetIntervals(), System.Math.Min(3, System.Math.Max(3, System.Math.Max(2, currentResearchTest.GetIntervals().Count - 2))));
        if (currentResearchTest.GetIntervals()[0].HasCurrentControl())
        {
            controlFitCoeffs = FindControlPolynomialLeastSquaresFit(currentResearchTest.GetIntervals(), System.Math.Min(2, System.Math.Max(3, System.Math.Max(2, currentResearchTest.GetIntervals().Count - 2))));
        } else
        {
            controlFitCoeffs = FindControlPolynomialLeastSquaresFit(currentResearchTest.GetIntervals(), 1);
        }



        controlAmt = currentResearchTest.GetControl();

        Debug.Log("Setting Up: " + currentTest + " | " + currentResearchTest.GetIntervals().Count + " intervals");

        

        startTime = currentResearchTest.GetIntervals()[0].GetTimeSinceSimStart();

        if (currentResearchTest.GetTestType() != "time")
        {
            if (startTime - guessAmount >= -273)
            {
                startTime -= guessAmount;
            } else
            {
                startTime = -273;
            }
            
        }

        currentTime = startTime;
        endTime = currentResearchTest.GetIntervals()[currentResearchTest.GetIntervals().Count - 1].GetTimeSinceSimStart() + guessAmount;

        
        timeSlider.minValue = startTime;
        timeSlider.maxValue = endTime;
        timeSlider.value = currentTime;

        startText.text = startTime + " " + currentResearchTest.GetIntervals()[0].GetUnits();
        endText.text = endTime + " " + currentResearchTest.GetIntervals()[currentResearchTest.GetIntervals().Count - 1].GetUnits();

        timeText.text = currentTime.ToString("0.000") + " " + currentResearchTest.GetIntervals()[0].GetUnits();

        amountText.text = currentResearchTest.GetComment();

        percentText.text = "Press Play to start";

        float amt = GetAmtValue(currentTime);
        if (amt < 0)
        {
            amt = 0;
        }
        DisplayAppropriateBacteria(amt);

        timeSlider.GetComponent<CustomSlider>().CreateFill(currentResearchTest, startTime, endTime);

        float max = Mathf.Max((float)GetAbsMax(bestFitCoeffs, startTime, endTime), (float)GetAbsMax(controlFitCoeffs, startTime, endTime));
        float add = Mathf.Min((float)GetMin(bestFitCoeffs, startTime, endTime), (float)GetMin(controlFitCoeffs, startTime, endTime));

        graph.Draw(currentResearchTest.GetIntervals(), bestFitCoeffs, startTime, endTime, max, add);
        controlGraph.Draw(currentResearchTest.GetIntervals(), controlFitCoeffs, startTime, endTime, max, add);

        graph.UpdateSlider(currentTime, startTime, endTime);

        Debug.Log("Done");
    }

    public double GetAbsMax(List<double> coeffs, float start, float end)
    {
        double max = 0;
        int iter = 0;
        for (float i = start; i < end; i += (end - start) / graph.GetVertexAmount())
        {
            if (iter >= graph.GetVertexAmount())
            {
                break;
            }
            float y = (float)F(coeffs, i);

            if (y > max)
            {
                max = y;
            }
        }
        return max;
    }

    public double GetMin(List<double> coeffs, float start, float end)
    {
        double min = double.MaxValue;
        int iter = 0;
        for (float i = start; i < end; i += (end - start) / graph.GetVertexAmount())
        {
            if (iter >= graph.GetVertexAmount())
            {
                break;
            }
            float y = (float)F(coeffs, i);

            if (y < min)
            {
                min = y;
            }
        }

        return min;
    }

    public void onSliderSet(float value)
    {
        currentTime = value;
        float amt = GetAmtValue(currentTime);
        if (amt < 0)
        {
            amt = 0;
        }
        DisplayAppropriateBacteria(amt);
        timeSlider.value = currentTime;
        timeText.text = currentTime.ToString("0.000") + " " + currentResearchTest.GetIntervals()[0].GetUnits();
        graph.UpdateSlider(currentTime, startTime, endTime);
    }

    public void OnPlayClicked()
    {
        if (isPaused)
        {
            Play();
        } else
        {
            Pause();
        }
    }

    private void Play()
    {
        isPaused = false;
        playButton.GetComponentInChildren<Text>().text = "Pause";
        StartCoroutine(ProgressTime());
    }

    private void Pause()
    {
        StopAllCoroutines();
        isPaused = true;
        playButton.GetComponentInChildren<Text>().text = "Play";
    }

    IEnumerator ProgressTime()
    {
        yield return new WaitForSeconds(1.0f/24.0f);
        currentTime += 1.0f/24.0f;

        timeSlider.value = currentTime;
        timeText.text = currentTime.ToString("0.000") + " " + currentResearchTest.GetIntervals()[0].GetUnits();

        float amt = GetAmtValue(currentTime);
        if (amt < 0)
        {
            amt = 0;
        }

        DisplayAppropriateBacteria(amt);

        graph.UpdateSlider(currentTime, startTime, endTime);

        if (currentTime >= endTime)
        {
            Pause();
            currentTime = startTime;
        } else
        {
            StartCoroutine(ProgressTime());
        }
    }

    private float GetAmtValue(float x)
    {

        float amt = 0f;
        for (int i = 0; i < bestFitCoeffs.Count; i++)
        {
            amt += (float)(bestFitCoeffs[i] * System.Math.Pow(x, i));
        }
        return amt;
    }

    private float GetControlValue(float x)
    {

        float amt = 0f;
        for (int i = 0; i < controlFitCoeffs.Count; i++)
        {
            amt += (float)(controlFitCoeffs[i] * System.Math.Pow(x, i));
        }

        return Mathf.Abs(amt);
    }

    public double F(List<double> coeffs, double x)
    {
        double total = 0;
        double x_factor = 1;
        for (int i = 0; i < coeffs.Count; i++)
        {
            total += x_factor * coeffs[i];
            x_factor *= x;
        }
        return total;
    }

    public double ErrorSquared(List<ResearchInterval> points, List<double> coeffs)
    {
        double total = 0;
        foreach (ResearchInterval pt in points)
        {
            double dy = pt.GetAmount() - F(coeffs, pt.GetTimeSinceSimStart());
            total += dy * dy;
        }
        return total;
    }

    // Find the least squares linear fit.
    public List<double> FindPolynomialLeastSquaresFit(
        List<ResearchInterval> points, int degree)
    {
        // Allocate space for (degree + 1) equations with 
        // (degree + 2) terms each (including the constant term).
        double[,] coeffs = new double[degree + 1, degree + 2];

        // Calculate the coefficients for the equations.
        for (int j = 0; j <= degree; j++)
        {
            // Calculate the coefficients for the jth equation.

            // Calculate the constant term for this equation.
            coeffs[j, degree + 1] = 0;
            foreach (ResearchInterval pt in points)
            {
                coeffs[j, degree + 1] -= System.Math.Pow(pt.GetTimeSinceSimStart(), j) * pt.GetAmount();
            }

            // Calculate the other coefficients.
            for (int a_sub = 0; a_sub <= degree; a_sub++)
            {
                // Calculate the dth coefficient.
                coeffs[j, a_sub] = 0;
                foreach (ResearchInterval pt in points)
                {
                    coeffs[j, a_sub] -= System.Math.Pow(pt.GetTimeSinceSimStart(), a_sub + j);
                }
            }
        }

        // Solve the equations.
        double[] answer = GaussianElimination(coeffs);

        // Return the result converted into a List<double>.
        return answer.ToList<double>();
    }


    // Find the least squares linear fit.
    public List<double> FindControlPolynomialLeastSquaresFit(
        List<ResearchInterval> points, int degree)
    {
        // Allocate space for (degree + 1) equations with 
        // (degree + 2) terms each (including the constant term).
        double[,] coeffs = new double[degree + 1, degree + 2];

        // Calculate the coefficients for the equations.
        for (int j = 0; j <= degree; j++)
        {
            // Calculate the coefficients for the jth equation.

            // Calculate the constant term for this equation.
            coeffs[j, degree + 1] = 0;
            foreach (ResearchInterval pt in points)
            {
                if (pt.HasCurrentControl())
                {
                    coeffs[j, degree + 1] -= System.Math.Pow(pt.GetTimeSinceSimStart(), j) * pt.GetCurrentControl();
                } else
                {
                    coeffs[j, degree + 1] -= System.Math.Pow(pt.GetTimeSinceSimStart(), j) * currentResearchTest.GetControl();
                }
                
            }

            // Calculate the other coefficients.
            for (int a_sub = 0; a_sub <= degree; a_sub++)
            {
                // Calculate the dth coefficient.
                coeffs[j, a_sub] = 0;
                foreach (ResearchInterval pt in points)
                {
                    coeffs[j, a_sub] -= System.Math.Pow(pt.GetTimeSinceSimStart(), a_sub + j);
                }
            }
        }

        // Solve the equations.
        double[] answer = GaussianElimination(coeffs);

        // Return the result converted into a List<double>.
        return answer.ToList<double>();
    }

    private double[] GaussianElimination(double[,] coeffs)
    {
        int max_equation = coeffs.GetUpperBound(0);
        int max_coeff = coeffs.GetUpperBound(1);
        for (int i = 0; i <= max_equation; i++)
        {
            // Use equation_coeffs[i, i] to eliminate the ith
            // coefficient in all of the other equations.

            // Find a row with non-zero ith coefficient.
            if (coeffs[i, i] == 0)
            {
                for (int j = i + 1; j <= max_equation; j++)
                {
                    // See if this one works.
                    if (coeffs[j, i] != 0)
                    {
                        // This one works. Swap equations i and j.
                        // This starts at k = i because all
                        // coefficients to the left are 0.
                        for (int k = i; k <= max_coeff; k++)
                        {
                            double temp = coeffs[i, k];
                            coeffs[i, k] = coeffs[j, k];
                            coeffs[j, k] = temp;
                        }
                        break;
                    }
                }
            }

            // Make sure we found an equation with
            // a non-zero ith coefficient.
            double coeff_i_i = coeffs[i, i];
            if (coeff_i_i == 0)
            {
                Debug.Log("There is no unique solution for these points. " + (coeffs.GetUpperBound(0) - 1) + " Setting to 1");
                coeffs[i, i] = 1;

            }

            // Normalize the ith equation.
            for (int j = i; j <= max_coeff; j++)
            {
                coeffs[i, j] /= coeff_i_i;
            }

            // Use this equation value to zero out
            // the other equations' ith coefficients.
            for (int j = 0; j <= max_equation; j++)
            {
                // Skip the ith equation.
                if (j != i)
                {
                    // Zero the jth equation's ith coefficient.
                    double coef_j_i = coeffs[j, i];
                    for (int d = 0; d <= max_coeff; d++)
                    {
                        coeffs[j, d] -= coeffs[i, d] * coef_j_i;
                    }
                }
            }
        }

        // At this point, the ith equation contains
        // 2 non-zero entries:
        //      The ith entry which is 1
        //      The last entry coeffs[max_coeff]
        // This means Ai = equation_coef[max_coeff].
        double[] solution = new double[max_equation + 1];
        for (int i = 0; i <= max_equation; i++)
        {
            solution[i] = coeffs[i, max_coeff];
        }

        // Return the solution values.
        return solution;
    }

private void DisplayAppropriateBacteria(float x)
    {
        controlAmt = GetControlValue(currentTime);

        float slope = startingBacteria / controlAmt;
        float amt = slope * x;

        int numBacteria = Mathf.RoundToInt(amt);

        for(int i = 0; i < bacteriaImages.Count; i++)
        {
            bacteriaImages[i].SetActive(i < numBacteria);
        }

        slope = startingBacteria / currentResearchTest.GetControl();
        amt = slope * controlAmt;

        numBacteria = Mathf.RoundToInt(amt);

        for (int i = 0; i < controlImages.Count; i++)
        {
            controlImages[i].SetActive(i < numBacteria);
        }

        percentText.text = ((x / controlAmt) * 100).ToString("0.000") + "% of control culture";
    }

    private void GenerateBacteriaImages()
    {
        

        for (int i = 0; i < maximumBacteria; i++)
        {
            GameObject bacteria = Instantiate(bacteriaPrefab);
            bacteria.transform.SetParent(petriDish.transform, false);

            var rectTransform = bacteria.GetComponent<RectTransform>();

            float radius = (petriDish.GetComponent<RectTransform>().rect.width / 2) - 32;

            Vector2 pos = new Vector2(0.5f, 0.5f);

            Vector3 target =  (radius * Random.insideUnitCircle);

            rectTransform.anchorMax = pos;
            rectTransform.anchorMin = pos;
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.sizeDelta = new Vector2(32, 32);

            rectTransform.rotation = Quaternion.Euler(0,0,Random.Range(0,360));
            rectTransform.localPosition = new Vector3(target.x, target.y, 0);

            bacteriaImages.Add(bacteria);

            if (i < startingBacteria)
            {
                bacteria.SetActive(true);
            } else
            {
                bacteria.SetActive(false);
            }

            GameObject controlBacteria = Instantiate(bacteriaPrefab);
            controlBacteria.transform.SetParent(controlPetriDish.transform, false);

            var controlRectTransform = controlBacteria.GetComponent<RectTransform>();

            float controlRadius = (controlPetriDish.GetComponent<RectTransform>().rect.width / 2) - 32;

            Vector2 controlPos = new Vector2(0.5f, 0.5f);

            Vector3 controlTarget = (controlRadius * Random.insideUnitCircle);

            controlRectTransform.anchorMax = controlPos;
            controlRectTransform.anchorMin = controlPos;
            controlRectTransform.offsetMax = Vector2.zero;
            controlRectTransform.offsetMin = Vector2.zero;
            controlRectTransform.sizeDelta = new Vector2(32, 32);

            controlRectTransform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
            controlRectTransform.localPosition = new Vector3(controlTarget.x, controlTarget.y, 0);

            controlImages.Add(controlBacteria);

            if (i < startingBacteria)
            {
                controlBacteria.SetActive(true);
            }
            else
            {
                controlBacteria.SetActive(false);
            }
        }

    }
}
