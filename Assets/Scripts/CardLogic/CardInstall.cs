using System;
public static class CardInstall {
    // play a card from hand, assuming it will succeed
    // installation must be completely reversible
    public static void Install(Card card) {
        InputOutput.Input(card);
        ZoneTracker.MoveCard(card,Zone.Hand,Zone.Play);
        AnimationHandler.Animate(card,GameAction.Installing);
        // wait for the player to select discards before outputting. Not necessary for reversible mode
        if (Game.S.ReversibleMode || DiscardRequester.S.pendingRequests==0) InputOutput.Output(card);
        if (!Game.S.ReversibleMode && card.scaleable) CardCopier.CreateCopy(card);
        Game.PlayerActionResolved();
    }
    // completely reverse an install
    // actions are undone in reverse order, compared to Install
    public static void Uninstall(Card card) {
        // uninstall parts above this one first
        Card above = Game.S.zoneTracker.playContents.GetAbove(card);
        if (above!=null) Uninstall(above);
        // delete temporary copies
        // let the copies be uninstalled first, so their inputs/outputs can be undone
        CardCopier.DeleteChild(card);
        // no need to undo output if the player is selecting discards, as the card has not output yet
        if (Game.S.ReversibleMode || DiscardRequester.S.pendingRequests==0) InputOutput.UndoOutput(card);
        ZoneTracker.MoveCard(card, Zone.Play, Zone.Hand);
        AnimationHandler.Animate(card, GameAction.Uninstalling);
        InputOutput.UndoInput(card);
        Game.PlayerActionResolved();
    }
    // reversibly installs and uninstalls the card to determine if it's possible to play
    public static bool CanInstall(Card card) {
        // if (!CardPlayable.isValid) then getting card.Playability calls CanInstall
        if (CardPlayable.isValid) {return (card.Playability==Playability.Playable);}
        // prevent animations and unnecessary repacking
        Game.StartReversibleMode();
        // install card
        InputOutput.Input(card);
        ZoneTracker.MoveCard(card,Zone.Hand,Zone.Play);
        // must be able to pay inputs before getting outputs
        bool canInstall = Validate.ValidState();
        // uninstall card
        ZoneTracker.MoveCard(card,Zone.Play,Zone.Hand);
        InputOutput.UndoInput(card);
        Game.EndReversibleMode();
        return canInstall;
    }
    // reversibly installs friends, then testCard, then uninstalls both
    // used to determine if a card could be played after playing another card first
    public static bool CanInstallWith(Card testCard,Card friend) {
        // no animations or irreversible actions during test
        Game.StartReversibleMode();

        InputOutput.Input(friend);
        ZoneTracker.MoveCard(friend,Zone.Hand,Zone.Play);
        // if playing friend requires discarding all your cards (including testCard), then it's not possible to install card
        bool testCardDiscarded = (ZoneTracker.availableDiscards == DiscardRequester.S.pendingRequests);
        if (!Validate.ValidState()) throw new Exception("Tried to CanInstallWith() using unplayable friend\nCard: "+testCard.cardName+"\nFriend: "+friend.cardName);
        InputOutput.Output(friend);

        bool canInstallTestCard = CanInstall(testCard);

        InputOutput.UndoOutput(friend);
        ZoneTracker.MoveCard(friend,Zone.Play,Zone.Hand);
        InputOutput.UndoInput(friend);

        Game.EndReversibleMode();
        return (canInstallTestCard && !testCardDiscarded);
    }
    public static void TryInstall(Card card) {
        // no installing cards during discard selection
        if (DiscardRequester.S.pendingRequests>0) throw new Exception("Tried to install card during discard selection");
        if (CanInstall(card)) Install(card);
    }
}
