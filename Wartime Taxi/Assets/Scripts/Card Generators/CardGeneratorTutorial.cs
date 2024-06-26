using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardGeneratorTutorial : CardGenerator
{

    [SerializeField]
    Card moveCard;
    [SerializeField]
    Card shootCard;
    [SerializeField]
    Card splitCard;
    [SerializeField]
    InputHandler inputHandler;
    [SerializeField]
    TutorialText tt;
    int counter = 0;
    public override Card generateCard() 
    {
        if (tt.currentPhase() < 5 && counter < 5)
        {
            moveCard.setInputHandler(this.inputHandler);
            counter += 1;
            return moveCard;
        }
        else if(tt.currentPhase() < 8)
        {
            shootCard.setInputHandler(this.inputHandler);
            return shootCard;

        }
        else if (tt.currentPhase() < 11)
        {

            int num1 = Random.Range(0, 1);
            counter += 1;
            if (num1 == 0)
            {
                moveCard.setInputHandler(this.inputHandler);
                return moveCard;
            }
            else
            {
                shootCard.setInputHandler(this.inputHandler);
                return shootCard;
            }

        }
        else if (tt.currentPhase() < 15)
        {

            splitCard.setInputHandler(this.inputHandler);
            counter = 0;
            return splitCard;
        }
        if (counter == 0) 
        {
            counter += 1;
            splitCard.setInputHandler(this.inputHandler);
            return splitCard;
        }
        int num = Random.Range(0, 10);
        if (num < 4)
        {
            moveCard.setInputHandler(this.inputHandler);
            return moveCard;
        }
        else if (num < 6) 
        { 
            splitCard.setInputHandler(this.inputHandler);
            return splitCard;
        }
        else
        {
            shootCard.setInputHandler(this.inputHandler);
            return shootCard;
        }
    }
}
