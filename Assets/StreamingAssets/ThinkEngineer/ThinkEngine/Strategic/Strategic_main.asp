playerBot_NoRecipeRequestCount(Count) :-
    Count = #count{ID : playerBot_HasNoRecipeRequest(ID)}.


recipeRequest_Count(Count) :-
    Count = #count{ID : recipeRequest_ID(ID)}.


recipeRequest_HasPlayer(RecipeRequestID) :-
    recipeRequest_ID(RecipeRequestID),
    playerBot_HasRecipeRequest(PlayerID),
    playerBot_RecipeRequest_ID(PlayerID, RecipeRequestID).

recipeRequest_HasNoPlayer(RecipeRequestID) :-
    recipeRequest_ID(RecipeRequestID),
    not recipeRequest_HasPlayer(RecipeRequestID).

recipeRequest_PlayerCount(RecipeRequestID, Count) :-
    recipeRequest_ID(RecipeRequestID),
    Count = #count{PlayerID : playerBot_RecipeRequest_ID(PlayerID, RecipeRequestID)}.

recipeRequest_PlayerCount(RecipeRequestID, 0) :-
    recipeRequest_ID(RecipeRequestID),
    recipeRequest_HasNoPlayer(RecipeRequestID).


recipeRequest_Player(RecipeRequestID, PlayerID) :-
    recipeRequest_ID(RecipeRequestID),
    recipeRequest_PlayerCount(RecipeRequestID, 1),
    playerBot_RecipeRequest_ID(PlayerID, RecipeRequestID).    




recipeRequestToAssign(RecipeRequestID) | recipeRequestToNotAssign(RecipeRequestID)  :-
    playerBot_NoRecipeRequestCount(Count),
    recipeRequest_HasNoPlayer(RecipeRequestID).


% a_SetRecipeRequest(ActionIndex, PlayerID, RecipeRequestID).


recipeRequestToAssign_Count(Count) :-
    Count = #count{RecipeRequestID : recipeRequestToAssign(RecipeRequestID)}.

:- recipeRequestToAssign_Count(R_Count),
    playerBot_NoRecipeRequestCount(P_Count),
    R_Count > P_Count.

{ recipeRequestToAssign_Player(RecipeRequestID, PlayerID) } <=1 :-
    recipeRequestToAssign(RecipeRequestID),
    playerBot_HasNoRecipeRequest(PlayerID).


:- recipeRequestToAssign_Player(RecipeRequestID, PlayerID), 
    recipeRequestToAssign_Player(RecipeRequestID, PlayerID2), 
    PlayerID != PlayerID2.

:- recipeRequestToAssign(RecipeRequestID),
    #count{PlayerID : recipeRequestToAssign_Player(RecipeRequestID, PlayerID)} != 1.

:- recipeRequestToAssign_Player(_, PlayerID),
    #count{RecipeRequestID : recipeRequestToAssign_Player(RecipeRequestID, PlayerID)} != 1.


recipeRequest_AllPrev(RecipeRequestID, PrevRecipeRequestID) :-
    recipeRequestToAssign(RecipeRequestID),
    recipeRequestToAssign(PrevRecipeRequestID),
    PrevRecipeRequestID < RecipeRequestID.

recipeRequest_Prev(RecipeRequestID, PrevRecipeRequestID) :-
    recipeRequestToAssign(RecipeRequestID),
    PrevRecipeRequestID = #max{PrevRecipeRequestID1 : recipeRequest_AllPrev(RecipeRequestID, PrevRecipeRequestID1) }.  




actionIndex_RecipeRequest(0, RecipeRequestID) :-
    recipeRequestToAssign(_),
    RecipeRequestID = #min{RecipeRequestID1 : recipeRequestToAssign(RecipeRequestID1)}.

actionIndex_RecipeRequest(ActionIndex, RecipeRequestID) :-
    ActionIndex = PrevActionIndex + 1,
    recipeRequest_Prev(RecipeRequestID, PrevRecipeRequestID),
    actionIndex_RecipeRequest(PrevActionIndex, PrevRecipeRequestID).



a_SetRecipeRequest(ActionIndex, PlayerID, RecipeRequestID) :-
    actionIndex_RecipeRequest(ActionIndex, RecipeRequestID),
    recipeRequestToAssign_Player(RecipeRequestID, PlayerID).


:~ recipeRequestToNotAssign(ID). [1@10, ID]


playerBot_MissingIngredients_ForRecipe(PlayerID, IngredientName, RecipeName) :-
    c_CompleteRecipe_Ingredient(RecipeName, IngredientName),
    playerBot_HasPlate(PlayerID),
    playerBot_Plate_ID(PlayerID, PlateID),
    not playerBot_IngredientsNames(PlayerID, IngredientName).

playerBot_HasInvalidIngredients_ForRecipe(PlayerID, RecipeName) :-
    playerBot_HasPlate(PlayerID),
    playerBot_Plate_ID(PlayerID, PlateID),
    playerBot_IngredientsNames(PlayerID, IngredientName),
    c_CompleteRecipe_Name(RecipeName),
    not c_CompleteRecipe_Ingredient(RecipeName, IngredientName).

playerBot_HasNoInvalidIngredients_ForRecipe(PlayerID, RecipeName) :-
    playerBot_HasPlate(PlayerID),
    playerBot_Plate_ID(PlayerID, PlateID),
    c_CompleteRecipe_Name(RecipeName),
    not playerBot_HasInvalidIngredients_ForRecipe(PlayerID, RecipeName).



#show applyAction/2.
#show actionArgument/3.
#show a_SetRecipeRequest/3.
#show recipeRequestToAssign_Count/1.
#show recipeRequestToAssign/1.
#show recipeRequestToNotAssign/1.
#show playerBot_ID/1.
#show playerBot_NoRecipeRequestCount/1.
#show playerBot_HasNoRecipeRequest/1.
#show recipeRequest_HasNoPlayer/1.
#show recipeRequest_HasPlayer/1.
#show recipeRequest_Player/2.
#show recipeRequest_PlayerCount/2.
#show recipeRequest/2.
#show recipeRequest_ID/1.
