
function TicTacToeCtrl($scope, $http) {

    var isTurn = false;
    var firstPlayerTurn = true;
    var gameIsRunning = false;
    var playerLetter = '';
    
    $scope.gameName   = '';
    $scope.playerName = '';
    $scope.rows = [
        [
            { 'id': 'A1', 'letter': '' },
            { 'id': 'B1', 'letter': '' },
            { 'id': 'C1', 'letter': '' }
        ],
        [
            { 'id': 'A2', 'letter': '' },
            { 'id': 'B2', 'letter': '' },
            { 'id': 'C2', 'letter': '' }
        ],
        [
            { 'id': 'A3', 'letter': '' },
            { 'id': 'B3', 'letter': '' },
            { 'id': 'C3', 'letter': '' }
        ]
    ];

    var startGameLoop = function () {
        setInterval($scope.refreshCallBack, 500);
    };

    var updateState = function (column) {

        if ($.trim(column.letter) != "")
            return;

        console.log('updating state...');

        column.letter = playerLetter;
        
        var newState = {
            gameName: $scope.gameName,
            firstPlayerTurn: firstPlayerTurn,
            rows: $scope.rows
        };

        $http.post('/home/UpdateGameState', newState)
            .success(function(data, status, headers, config) {
                column.letter = playerLetter;
                if (checkWin())
                    alert('You won!');
                
                isTurn = false;
                console.log('success!');
                console.log(data);
            })
            .error(function(data, status, headers, config) {
                console.log('error');
                if (status == 400) {
                    column.letter = '';
                    alert('Waiting for second player...');
                }
            });
    };

    var checkWin = function () {

        var letter = playerLetter;

        var textToLookFor = letter + letter + letter;
        for (var count = 0; count < $scope.rows.length; count++) {
            if ($scope.rows[count][0].letter + $scope.rows[count][1].letter + $scope.rows[count][2].letter == textToLookFor) {

                return true;
            }
            if ($scope.rows[0][count].letter + $scope.rows[1][count].letter + $scope.rows[2][count].letter == textToLookFor) {

                return true;
            }
        }
        if ($scope.rows[0][0].letter + $scope.rows[1][1].letter + $scope.rows[2][2].letter == textToLookFor) {

            return true;
        }
        if ($scope.rows[2][0].letter + $scope.rows[1][1].letter + $scope.rows[0][2].letter == textToLookFor) {

            return true;
        }
        return false;
    };

    $scope.refreshCallBack = function () {
        $http.get('/home/getcurrentstate?gameName=' + $scope.gameName + '&playerName=' + $scope.playerName)
                .success(function (data, status, headers, config) {
                    console.log(data);

                    if ($.trim(data) == "" || data.rows.length == 0)
                        return;

                    isTurn = data.isTurn;
                    $scope.rows = data.rows;
                })
                .error(function (data, status, headers, config) {
                    console.log('Failed to get state');
                });

        console.log('Retrieving state...');
    };

    $scope.startOrJoinGame = function() {
        $http.post('/home/initgame', { gameName: $scope.gameName, playerName: $scope.playerName })
             .success(function (data, status, headers, config) {
                 console.log('success!');
                 console.log(data);

                 if(data.isNewGame) {
                     playerLetter = 'X';//data.session.Player1.PlayerLetter;
                     isTurn = true;
                 } else {
                     playerLetter = 'O';//data.session.Player2.PlayerLetter;
                 }

                 startGameLoop();

                 gameIsRunning = true;
             })
            .error(function (data, status, headers, config) {
                console.log('error');
            });
    };

    $scope.markUserClick = function (column) {
        if (!gameIsRunning) {
            alert('Please start or join a game!');
            return;
        }

        if (!isTurn) {
            alert('Please wait your turn');
            return;
        }

        console.log('clicked column: ' + column.id);

        updateState(column);
    };
}