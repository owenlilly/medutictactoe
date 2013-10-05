
function TicTacToeCtrl($scope, $http) {

    var refreshInterval;
    var firstPlayerTurn = true;
    var playerLetter = '';
    var gameOver = false;

    $scope.userState = {
        gameOver: false,
        winCount: 0,
        winnerName: '',
        playerName: '',
        gameName: '',
        isTurn: false,
        rows: $scope.rows
    };

    $scope.gameIsRunning = false;
    $scope.turnName = $scope.gameIsRunning ? ($scope.userState.isTurn ? 'Your turn' : $scope.oppName+'\'s turn') : '';
    $scope.controlsDisabled = false;
    $scope.myScore = 0;
    $scope.oppScore = 0;
    $scope.gameName   = '--';
    $scope.playerName = '--';
    $scope.secondPlayerName = '--';
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
        refreshInterval = setInterval($scope.refreshCallBack, 500);
    };

    var updateState = function(column) {

        if ($.trim(column.letter) != "")
            return;

        console.log('updating state...');

        column.letter = playerLetter;

        $scope.userState = {
            gameName: $scope.gameName,
            playerName: $scope.playerName,
            isTurn: firstPlayerTurn,
            rows: $scope.rows
        };

        $http.post('/home/UpdateGameState', $scope.userState)
            .success(function(data, status, headers, config) {
                gameOver = $scope.userState.gameOver = data.gameOver;
                $scope.userState.isTurn = data.isTurn;
            })
            .error(function(data, status, headers, config) {
                console.log('error');
                if (status == 400) {
                    column.letter = '';
                    alert('Waiting for second player...');
                }
            });
    };

    $scope.refreshCallBack = function () {

        $http.get('/home/getcurrentstate?gameName=' + $scope.gameName + '&playerName=' + $scope.playerName)
                .success(function (data, status, headers, config) {
                    console.log(data);

                    if ($.trim(data) == "" || data.rows.length == 0)
                        return;

                    if (data.gameOver) {
                        if ($.trim(data.winnerName) == $.trim($scope.playerName)) {
                            $scope.rows = data.rows;
                            alert('You won!');
                        } else {
                            $scope.rows = data.rows;
                            alert('You lost!');
                        }
                        gameOver = $scope.userState.gameOver = true;
                        clearInterval(refreshInterval);
                        return;
                    }
                    $scope.userState.isTurn = data.isTurn;
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
                     playerLetter = data.session.Player1.PlayerLetter;
                    
                     $scope.userState.isTurn = true;
                 } else {
                     playerLetter = data.session.Player2.PlayerLetter;

                 }
                 
                 startGameLoop();

                 $scope.controlsDisabled = true;
                 $scope.gameIsRunning = true;
             })
            .error(function (data, status, headers, config) {
                console.log('error');
            });
    };

    $scope.markUserClick = function (column) {
        if ($scope.userState.gameOver) {
            alert('Game over');
            return;
        }

        if (!$scope.gameIsRunning) {
            alert('Please start or join a game!');
            return;
        }

        if (!$scope.userState.isTurn) {
            alert('Please wait your turn');
            return;
        }

        console.log('clicked column: ' + column.id);

        updateState(column);
    };
}