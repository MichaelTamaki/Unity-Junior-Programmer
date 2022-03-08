using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Subclass of Unit that will improve the production speed of a resource pile
/// </summary>
public class ProductivityUnit : Unit
{
    public float ProductivityMultiplier = 2;
    private ResourcePile m_CurrentPile = null;

    private void ResetPileProductivity()
    {
        if (m_CurrentPile != null)
        {
            m_CurrentPile.ProductionSpeed /= ProductivityMultiplier;
            m_CurrentPile = null;
        }
    }

    public override void GoTo(Vector3 position)
    {
        ResetPileProductivity();
        base.GoTo(position);
    }

    public override void GoTo(Building target)
    {
        ResetPileProductivity();
        base.GoTo(target);
    }

    protected override void BuildingInRange()
    {
        if (m_CurrentPile == null)
        {
            ResourcePile pile = m_Target as ResourcePile;
            if (pile != null)
            {
                m_CurrentPile = pile;
                m_CurrentPile.ProductionSpeed *= ProductivityMultiplier;
            }
        }
    }
    
    //Override all the UI function to give a new name and display what it is currently transporting
    public override string GetName()
    {
        return "Productivity Unit";
    }

    public override string GetData()
    {
        return $"Improves resource pile production rate by {ProductivityMultiplier}";
    }
}
