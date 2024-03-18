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
            if(counter == 3) 
            {
                shootCard.setInputHandler(this.inputHandler);
                return shootCard;
            }
            return moveCard;
        }
        else if(tt.currentPhase() < 5)
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
            return splitCard;
        }

        int num = Random.Range(0, 10);
        if (num < 4)
        {
            moveCard.setInputHandler(this.inputHandler);
            return moveCard;
        }
        else if (num < 7) 
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
