<script>
    // @ts-nocheck
    
    import { characters, sets } from '../../lib/unmatchedData.js';
    
    let characterStatuses = [
        ["Medusa", true], ["Alice", true], ["King Arthur", true], ["Sinbad", true], ["Bloody Mary", true],
        ["Achilles", true], ["Yennenga", true], ["Sun Wukong", true], ["Sherlock Holmes", true], ["Invisible Man", true],
        ["Dracula", true], ["Jekyll & Hyde", true], ["Buffy", true], ["Spike", true], ["Willow", true],
        ["Angel", true], ["Nikola Tesla", true], ["Golden Bat", true], ["Jill Trent", true], ["Annie Christmas", true],
        ["William Shakespeare", true], ["Titania", true], ["Hamlet", true], ["Wayward Sisters", true], ["Luke Cage", true],
        ["Moon Knight", true], ["Ghost Rider", true], ["Elektra", true], ["Daredevil", true], ["Bullseye", true],
        ["Squirrel Girl", true], ["Ms Marvel", true], ["Cloak and Dagger", true], ["Black Widow", true], ["Black Panther", true],
        ["Winter Soldier", true], ["She-Hulk", true], ["Spiderman", true], ["Dr Strange", true], ["Robin Hood", true],
        ["Bigfoot", true], ["InGen", true], ["Raptors", true], ["Little Red Riding Hood", true], ["Beowulf", true],
        ["Dr Ellie Sattler", true], ["T-Rex", true], ["Houdini", true], ["The Genie", true], ["Oda Nobunaga", true],
        ["Tomoe Gozen", true], ["Bruce Lee", true], ["Deadpool", true]
    ];
    
    let roundThreeMatchups = [];
    let roundTwoMatchups = [];
    let roundOneMatchups = [];
    let quarterfinalMatchups = [];
    let semifinalMatchups = [];
    let finalMatchups = [];
    
    // Define maximum matchups for each round
    const maxMatchupsPerRound = {
        roundOne: 32,
        roundTwo: 16,
        roundThree: 8,
        quarterfinal: 4,
        semifinal: 2,
        final: 1
    };
    
    // Placeholder name
    const PLACEHOLDER = "BYE";
    
    function GetCharacterIndex(name) {
        for (let i = 0; i < characterStatuses.length; i++) {
            if (name === characterStatuses[i][0]) {
                return i;
            }
        }
        return -1;
    }
    
    function UpdateCharacterStatus(event, character) {
        const index = GetCharacterIndex(character);
        if (index !== -1) {
            characterStatuses[index][1] = event.target.checked;
        }
    }
    
    function GetMatchupCharacter(name) {
        for (let index = 0; index < characters.length; index++) {
            if (characters[index].name === name) {
                return characters[index];
            }
        }
        return { name: "Deadpool", img: "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTBomL1APow7XzyTYWzc4PG2yeqwfXj8S5MLA&s" };
    }
    
    function GetMatchupImage(name) {
        let character = GetMatchupCharacter(name);
        return character.img;
    }
    
    function AdvanceCharacter(character, currentRound, nextRound) {
        console.log(nextRound);
        if (character === PLACEHOLDER) return;  // Placeholders cannot be advanced
    
        const index = currentRound.findIndex(match => match.includes(character));
        if (index !== -1) {
            if (nextRound.length < maxMatchupsPerRound[getRoundKey(nextRound)] || getRoundKey(nextRound) === 'final') {
                if (nextRound[nextRound.length - 1]?.length === 1) {
                    nextRound[nextRound.length - 1].push(character);
                } else {
                    nextRound.push([character]);
                }
                currentRound.splice(index, 1);
            }else if(nextRound.length = maxMatchupsPerRound[getRoundKey(nextRound)] && nextRound[nextRound.length].length != 2){
                nextRound[nextRound.length - 1].push(character);
            }
        }
        console.log(nextRound);
    
        roundOneMatchups = [...roundOneMatchups];
        roundTwoMatchups = [...roundTwoMatchups];
        roundThreeMatchups = [...roundThreeMatchups];
        quarterfinalMatchups = [...quarterfinalMatchups];
        semifinalMatchups = [...semifinalMatchups];
        finalMatchups = [...finalMatchups];
    }
    
    function GetColor(set, character) {
        const characterIndex = set.Characters.indexOf(character);
        return set.Colors[characterIndex];
    }
    
    function CreateTournament() {
        let activeCharacters = [];
        for (let i = 0; i < characterStatuses.length; i++) {
            if (characterStatuses[i][1]) {
                activeCharacters.push(characterStatuses[i][0]);
            }
        }
    
        // Add placeholders to reach the nearest power of two
        let nearestPower = nearestPowerOfTwo(activeCharacters.length);
        while (activeCharacters.length < nearestPower) {
            activeCharacters.push(PLACEHOLDER);
        }
    
        // Initialize round arrays
        roundThreeMatchups = [];
        roundTwoMatchups = [];
        roundOneMatchups = [];
        quarterfinalMatchups = [];
        semifinalMatchups = [];
        finalMatchups = [];
    
        ShuffleArray(activeCharacters);
    
        // Generate initial matchups ensuring placeholders are not matched against each other
        roundOneMatchups = generateMatchups(activeCharacters);
    
        // Ensure each round array is set correctly to trigger Svelte reactivity
        roundOneMatchups = [...roundOneMatchups];
        roundTwoMatchups = [...roundTwoMatchups];
        roundThreeMatchups = [...roundThreeMatchups];
        quarterfinalMatchups = [...quarterfinalMatchups];
        semifinalMatchups = [...semifinalMatchups];
        finalMatchups = [...finalMatchups];
    }
    
    function nearestPowerOfTwo(n) {
        return Math.pow(2, Math.ceil(Math.log2(n)));
    }
    
    function ShuffleArray(array) {
        for (let i = array.length - 1; i > 0; i--) {
            const j = Math.floor(Math.random() * (i + 1));
            [array[i], array[j]] = [array[j], array[i]];
        }
    }
    
    function generateMatchups(participants) {
        console.log(participants.length);
        let matchups = [];
        let realParticipants = participants.filter(p => p !== PLACEHOLDER);
        let placeholders = participants.filter(p => p === PLACEHOLDER);

        for (let i = 0; i < placeholders.length; i++) {
            matchups.push([realParticipants.pop(),placeholders.pop()]);
            
        }
        while (realParticipants.length > 1) {
            matchups.push([realParticipants.pop(), realParticipants.pop()]);
        }


        console.log("Generated matchups:", matchups);
        return matchups;
    }

    
    function getRoundKey(roundArray) {
        switch (roundArray) {
            case roundOneMatchups:
                return 'roundOne';
            case roundTwoMatchups:
                return 'roundTwo';
            case roundThreeMatchups:
                return 'roundThree';
            case quarterfinalMatchups:
                return 'quarterfinal';
            case semifinalMatchups:
                return 'semifinal';
            case finalMatchups:
                return 'final';
            default:
                return '';
        }
    }
    
    function SetSetId(set) {
        let ID = set.Characters.length;
        let StringID = "";
        switch (ID) {
            case 1:
                StringID = "One";
                break;
            case 2:
                StringID = "Two";
                break;
            case 3:
                StringID = "Three";
                break;
            case 4:
            default:
                StringID = "Four";
                break;
        }
        return StringID;
    }
</script>
    
    
    
    
    
    
    
    

<main>
    <div style="display: flex; flex-direction: row; overflow-x: auto; white-space: nowrap;">
        {#each sets as set}
            <div class="set" id={SetSetId(set)}>
                <div style="min-height: 235px;">
                    <div style="display: flex; flex-direction: column; justify-content: center; align-items: center;">
                        <img src={set.Image} alt={set.Name} style="max-width: 170px; max-height: 220px; margin-top:10px;">
                    </div>
                    <h4 style="text-align:center; width: 100%; margin-top: auto;">{set.Name}</h4>
                </div>
                <ol>
                    {#each set.Characters as character}
                        {#if GetCharacterIndex(character) !== -1}
                            <li style="display: flex; justify-content: space-between; align-items: center;">
                                <span style="color:{GetColor(set, character)}">{character}</span>
                                <input type="checkbox" 
                                    bind:checked={characterStatuses[GetCharacterIndex(character)][1]} 
                                    on:change={(event) => UpdateCharacterStatus(event, character)}>
                            </li>
                        {/if}
                    {/each}
                </ol>
            </div>
        {/each}
    </div>
    <button style="height: 70px; width:120px; padding:10px; margin-top:15px; background-color:limegreen; border-radius:5px; color:green; font-size:large;" on:click={CreateTournament}>Create Tournament</button>

    <h1 style="margin-top:20px; text-decoration:double; font-size:larger; margin-bottom:5px;">"Round of 64"</h1>
    <div style="display: flex; flex-direction: row; overflow-x: auto; white-space: nowrap;">
        {#each roundOneMatchups as matchup}
            <div class="matchup">
                <div class="matchup-set">
                    <div style="min-height: 235px; margin-right:10px">
                        <div style="display: flex; flex-direction: column; justify-content: center; align-items: center;">
                            <img src={GetMatchupImage(matchup[0])} alt="{matchup[0]}" style="max-width: 170px; max-height: 220px; margin-top:10px;">
                        </div>
                        <h4 style="text-align:center; width: 100%; margin-top: auto;">{matchup[0]}</h4>
                        <div class="advance-container">
                            <button class="advance-button" on:click={() => AdvanceCharacter(matchup[0], roundOneMatchups, roundTwoMatchups)}>Advance</button>
                        </div>
                    </div>
                </div>
                {#if matchup[1]}
                <div class="matchup-set">
                    <div style="min-height: 235px;">
                        <div style="display: flex; flex-direction: column; justify-content: center; align-items: center;">
                            <img src={GetMatchupImage(matchup[1])} alt="{matchup[1]}" style="max-width: 170px; max-height: 220px; margin-top:10px;">
                            <h4 style="text-align:center; width: 100%; margin-top: auto;">{matchup[1]}</h4>
                        </div>
                        <div class="advance-container">
                            <button class="advance-button" on:click={() => AdvanceCharacter(matchup[1], roundOneMatchups, roundTwoMatchups)}>Advance</button>
                        </div>
                    </div>
                </div>
                {/if}
            </div>
        {/each}
    </div>

    <h1 style="margin-top:20px; text-decoration:double; font-size:larger; margin-bottom:5px;">"Round of 32"</h1>
    <div style="display: flex; flex-direction: row; overflow-x: auto; white-space: nowrap;">
        {#each roundTwoMatchups as matchup}
            <div class="matchup">
                <div class="matchup-set">
                    <div style="min-height: 235px; margin-right:10px">
                        <div style="display: flex; flex-direction: column; justify-content: center; align-items: center;">
                            <img src={GetMatchupImage(matchup[0])} alt="{matchup[0]}" style="max-width: 170px; max-height: 220px; margin-top:10px;">
                        </div>
                        <h4 style="text-align:center; width: 100%; margin-top: auto;">{matchup[0]}</h4>
                        <div class="advance-container">
                            <button class="advance-button" on:click={() => AdvanceCharacter(matchup[0], roundTwoMatchups, roundThreeMatchups)}>Advance</button>
                        </div>
                    </div>
                </div>
                {#if matchup[1]}
                <div class="matchup-set">
                    <div style="min-height: 235px;">
                        <div style="display: flex; flex-direction: column; justify-content: center; align-items: center;">
                            <img src={GetMatchupImage(matchup[1])} alt="{matchup[1]}" style="max-width: 170px; max-height: 220px; margin-top:10px;">
                            <h4 style="text-align:center; width: 100%; margin-top: auto;">{matchup[1]}</h4>
                        </div>
                        <div class="advance-container">
                            <button class="advance-button" on:click={() => AdvanceCharacter(matchup[1], roundTwoMatchups, roundThreeMatchups)}>Advance</button>
                        </div>
                    </div>
                </div>
                {/if}
            </div>
        {/each}
    </div>

    <h1 style="margin-top:20px; text-decoration:double; font-size:larger; margin-bottom:5px;">"Round of 16"</h1>
    <div style="display: flex; flex-direction: row; overflow-x: auto; white-space: nowrap;">
        
        {#each roundThreeMatchups as matchup}
            <div class="matchup">
                <div class="matchup-set">
                    <div style="min-height: 235px; margin-right:10px">
                        <div style="display: flex; flex-direction: column; justify-content: center; align-items: center;">
                            <img src={GetMatchupImage(matchup[0])} alt="{matchup[0]}" style="max-width: 170px; max-height: 220px; margin-top:10px;">
                        </div>
                        <h4 style="text-align:center; width: 100%; margin-top: auto;">{matchup[0]}</h4>
                        <div class="advance-container">
                            <button class="advance-button" on:click={() => AdvanceCharacter(matchup[0], roundThreeMatchups, quarterfinalMatchups)}>Advance</button>
                        </div>
                    </div>
                </div>
                {#if matchup[1]}
                <div class="matchup-set">
                    <div style="min-height: 235px;">
                        <div style="display: flex; flex-direction: column; justify-content: center; align-items: center;">
                            <img src={GetMatchupImage(matchup[1])} alt="{matchup[1]}" style="max-width: 170px; max-height: 220px; margin-top:10px;">
                            <h4 style="text-align:center; width: 100%; margin-top: auto;">{matchup[1]}</h4>
                        </div>
                        <div class="advance-container">
                            <button class="advance-button" on:click={() => AdvanceCharacter(matchup[1], roundThreeMatchups, quarterfinalMatchups)}>Advance</button>
                        </div>
                    </div>
                </div>
                {/if}
            </div>
        {/each}
    </div>

    <h1 style="margin-top:20px; text-decoration:double; font-size:larger; margin-bottom:5px;">"Quarterfinals"</h1>
    <div style="display: flex; flex-direction: row; overflow-x: auto; white-space: nowrap;">
        {#each quarterfinalMatchups as matchup}
            <div class="matchup">
                <div class="matchup-set">
                    <div style="min-height: 235px; margin-right:10px">
                        <div style="display: flex; flex-direction: column; justify-content: center; align-items: center;">
                            <img src={GetMatchupImage(matchup[0])} alt="{matchup[0]}" style="max-width: 170px; max-height: 220px; margin-top:10px;">
                        </div>
                        <h4 style="text-align:center; width: 100%; margin-top: auto;">{matchup[0]}</h4>
                        <div class="advance-container">
                            <button class="advance-button" on:click={() => AdvanceCharacter(matchup[0], quarterfinalMatchups, semifinalMatchups)}>Advance</button>
                        </div>
                    </div>
                </div>
                {#if matchup[1]}
                <div class="matchup-set">
                    <div style="min-height: 235px;">
                        <div style="display: flex; flex-direction: column; justify-content: center; align-items: center;">
                            <img src={GetMatchupImage(matchup[1])} alt="{matchup[1]}" style="max-width: 170px; max-height: 220px; margin-top:10px;">
                            <h4 style="text-align:center; width: 100%; margin-top: auto;">{matchup[1]}</h4>
                        </div>
                        <div class="advance-container">
                            <button class="advance-button" on:click={() => AdvanceCharacter(matchup[1], quarterfinalMatchups, semifinalMatchups)}>Advance</button>
                        </div>
                    </div>
                </div>
                {/if}
            </div>
        {/each}
    </div>

    <h1 style="margin-top:20px; text-decoration:double; font-size:larger; margin-bottom:5px;">"Semifinals"</h1>
    <div style="display: flex; flex-direction: row; overflow-x: auto; white-space: nowrap;">
        {#each semifinalMatchups as matchup}
            <div class="matchup">
                <div class="matchup-set">
                    <div style="min-height: 235px; margin-right:10px">
                        <div style="display: flex; flex-direction: column; justify-content: center; align-items: center;">
                            <img src={GetMatchupImage(matchup[0])} alt="{matchup[0]}" style="max-width: 170px; max-height: 220px; margin-top:10px;">
                        </div>
                        <h4 style="text-align:center; width: 100%; margin-top: auto;">{matchup[0]}</h4>
                        <div class="advance-container">
                            <button class="advance-button" on:click={() => AdvanceCharacter(matchup[0], semifinalMatchups, finalMatchups)}>Advance</button>
                        </div>
                    </div>
                </div>
                {#if matchup[1]}
                <div class="matchup-set">
                    <div style="min-height: 235px;">
                        <div style="display: flex; flex-direction: column; justify-content: center; align-items: center;">
                            <img src={GetMatchupImage(matchup[1])} alt="{matchup[1]}" style="max-width: 170px; max-height: 220px; margin-top:10px;">
                            <h4 style="text-align:center; width: 100%; margin-top: auto;">{matchup[1]}</h4>
                        </div>
                        <div class="advance-container">
                            <button class="advance-button" on:click={() => AdvanceCharacter(matchup[1], semifinalMatchups, finalMatchups)}>Advance</button>
                        </div>
                    </div>
                </div>
                {/if}
            </div>
        {/each}
    </div>

    <h1 style="margin-top:20px; text-decoration:double; font-size:larger; margin-bottom:5px;">"Finals"</h1>
    <div style="display: flex; flex-direction: row; overflow-x: auto; white-space: nowrap;">
        {#each finalMatchups as matchup}
            <div class="matchup">
                <div class="matchup-set">
                    <div style="min-height: 235px; margin-right:10px">
                        <div style="display: flex; flex-direction: column; justify-content: center; align-items: center;">
                            <img src={GetMatchupImage(matchup[0])} alt="{matchup[0]}" style="max-width: 170px; max-height: 220px; margin-top:10px;">
                        </div>
                        <h4 style="text-align:center; width: 100%; margin-top: auto;">{matchup[0]}</h4>
                        <div class="advance-container">
                            <button class="advance-button" on:click={() => AdvanceCharacter(matchup[0], finalMatchups, [])}>Advance</button>
                        </div>
                    </div>
                </div>
                {#if matchup[1]}
                <div class="matchup-set">
                    <div style="min-height: 235px;">
                        <div style="display: flex; flex-direction: column; justify-content: center; align-items: center;">
                            <img src={GetMatchupImage(matchup[1])} alt="{matchup[1]}" style="max-width: 170px; max-height: 220px; margin-top:10px;">
                            <h4 style="text-align:center; width: 100%; margin-top: auto;">{matchup[1]}</h4>
                        </div>
                        <div class="advance-container">
                            <button class="advance-button" on:click={() => AdvanceCharacter(matchup[1], finalMatchups, [])}>Advance</button>
                        </div>
                    </div>
                </div>
                {/if}
            </div>
        {/each}
    </div>

    <div style="height: 300px;"></div>
</main>



<style>
body, html {
    height: 100%;
    margin: 0;
    font-family: Arial, sans-serif;
}

body {
    overflow-y: auto;
    background-color: #f0f0f0;
}

main {
    padding: 20px;
}

.set {
    padding: 13px;
    margin: 10px;
    width: 200px;
    display: flex;
    flex-direction: column;
    align-items: center;
    background-color: black;
    overflow: hidden;
    transition: all 0.5s ease;
    height: 300px;
    position: relative;
    flex-shrink: 0;
}

.matchup {
    display: flex;
    flex-direction: row;
    margin-right: 20px;
    background-color: black;
    padding: 10px;
    transition: all 0.5s ease;
    height: 250px; /* Initial height */
    overflow: hidden;
    position: relative;
    flex-shrink: 0;
}

.matchup:hover {
    height: 370px; /* Expanded height */
}

.matchup-set {
    position: relative;
    transition: max-height 0.5s ease;
    max-height: 250px; /* Initial max height */
    overflow: hidden;
}

.matchup:hover .matchup-set {
    max-height: 360px; /* Expanded max height */
}

.advance-container {
    position: relative;
    width: 100%;
    display: flex;
    justify-content: center;
    align-items: flex-end;
    height: 100%;
    margin-top: 10px;
}

.advance-button {
    opacity: 0;
    transition: opacity 0.5s ease, all 0.5s ease;
    position: absolute;
    top: 10px;
    left: 50%;
    transform: translateX(-50%);
    background-color: limegreen;
    color: darkgreen;
    font-size: large;
    padding: 10px;
    border-radius: 5px;
}

.matchup:hover .advance-button {
    opacity: 1;
    top: 20px;
}

#One:hover, #One[aria-expanded="true"] { height: 330px; }
#Two:hover, #Two[aria-expanded="true"] { height: 350px; }
#Three:hover, #Three[aria-expanded="true"] { height: 370px; }
#Four:hover, #Four[aria-expanded="true"] { height: 390px; }

.set:hover h4, .set[aria-expanded="true"] { color: Grey; }

.set > div {
    flex: 1;
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    width: 100%;
}

.set ol {
    position: absolute;
    top: 0;
    left: 0;
    width: 100%;
    transition: transform 0.5s ease, opacity 0.5s ease;
    transform: translateY(100%);
    opacity: 0;
    margin: 0;
    padding: 0 13px;
    margin-top: 250px;
    list-style: none;
    background: rgba(0, 0, 0, 0.8);
}

.set:hover ol, .set[aria-expanded="true"] {
    transform: translateY(0);
    opacity: 1;
}

.set h4 {
    text-align: center;
    width: 100%;
    margin-top: auto;
}
</style>
