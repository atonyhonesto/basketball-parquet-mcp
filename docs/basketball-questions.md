# Thirty player focused basketball questions

Adapted from the original basketball exploration catalogue. This is a set of analytical prompts, not thirty implemented tools or claimed model results. Use player IDs and define tie handling, eligibility and missing-game behavior before answering. Public fixtures use fictional names.

## Scoring

1. Who led all players in total points across the series?
2. Which player had the most games with at least 20 points?
3. Who had the highest single-game scoring performance?
4. Which bench player averaged the most points per game played?
5. Who had the lowest game-to-game scoring variance among eligible players?
6. Which starters scored fewer points than their team's top bench scorer most often?

## Efficiency and shooting

7. Who had the best field-goal percentage among players meeting a declared attempts-per-game threshold?
8. Who made the most three-pointers, and at what percentage?
9. Who had the best series free-throw percentage above a declared minimum attempt count?
10. Who had the largest efficiency decline from the first game to the final game?

For questions 7–9, combine made/attempted totals before calculating percentages. Question 9 has a dedicated deterministic tool in this public edition.

## Rebounding

11. Who led in total rebounds?
12. Which guard collected the most rebounds?
13. Who had the most offensive rebounds?

## Playmaking

14. Who had the most assists?
15. Who had the best assist-to-turnover ratio, with an explicit zero-turnover rule?
16. Who had the most assists in a single game?

## Defense

17. Who led the series in total steals?
18. Who had the most blocks?
19. Which players reached at least three steals and three blocks across the series?

## Plus minus

20. Who had the best cumulative plus/minus?
21. Who had the largest spread between their best and worst single-game plus/minus?
22. Which bench player had the highest total plus/minus?

Plus/minus is contextual. These questions do not establish individual causal impact.

## Player comparisons

23. How did two selected players' scoring compare game by game?
24. How did two selected players compare in points plus rebounds plus assists?
25. Which player's averages differed most between team wins and losses?

## Consistency and team results

26. Who scored in double figures in every game they played?
27. Who had the most games below eight points while their team still won?
28. Whose performance most closely tracked team wins and losses, under a declared association metric?

A short series is a small sample. Question 28 is exploratory, not a causal conclusion. These are box-score questions; genuine clutch analysis requires event timing and game-state data.

## Role players

29. Which non-starter had the most total points plus rebounds plus assists?
30. Who had the largest gap between their best and worst scoring games?
