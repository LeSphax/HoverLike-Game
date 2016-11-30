var nickname = "Leroy Jenkins";
var unity_ready = false;
var startGame = false;
var isChrome = !!window.chrome && !!window.chrome.webstore;

var cookie_nickname = Cookies.get('nickname');
var cookie_inputs = Cookies.get('inputs');

$(function() {

  if (!isChrome){
    $("#configuration_content").empty();
    $("#configuration_content").html('At the moment, the game only works on Google chrome. ' +
      'Please change your browser or download the <a href="https://github.com/LeSphax/HoverLike-Game/raw/PC/Slideball.zip">PC version</a>.');
  }
/*
 resizeCanvas();
 $(window).on('resize orientationChange', function(event) {
  resizeCanvas();
});*/

if (cookie_nickname){
  console.log("Nickname was found " + cookie_nickname);
  $("#nickname_input").val(cookie_nickname);
}

if (cookie_inputs){
  console.log("Inputs were found " + cookie_inputs);
  for(var i = 0 ; i<cookie_inputs.length; i++){
    if (cookie_inputs[i] == " ")
      $("#input_"+i).html("Space");
    else
      $("#input_"+i).html(cookie_inputs[i]);
  }
}

$(".input_button").each(function(){
  $(this).click(function(){
    console.log($(this) + "clicked");
    var button = $(this);
    button.html('');
    button.focus();
    button.keydown(function(event){
      if (event.key == " "){
        button.html("Space");
      }
      else{
        button.html(event.key.toUpperCase());
      }
      event.preventDefault();
      event.stopPropagation();
      button.off('keypress');
    })
  });
});
});

function resizeCanvas(){
  $("#canvas").attr('width', $(document).width());
  $("#canvas").attr('height', $(document).height());
}

function startPlaying(){
  nickname = $("#nickname_input").val() ? $("#nickname_input").val() : "Leroy Jenkins";
  Cookies.set('nickname', nickname);
  $("#configuration").css('display','none');
  $("#unity-player").css('display','inline');
  startGame = true;
  TryStartGame();

};

function UnityIsReady(){
  unity_ready = true;
  TryStartGame();
};

function TryStartGame(){
  if (unity_ready && startGame){
    SendMessage("NetworkScripts", "SetNickname", nickname);
    SendMessage("NetworkScripts", "SetConfiguration", GetConfiguration());
  }
}

function GetConfiguration(){

  var inputs="";
  for (var i =0; i<5; i++){
    var value = $("#input_"+i).html();
    if (value == "Space"){
      inputs+= " ";
    }
    else{
      inputs += $("#input_"+i).html();
    }
  }
  Cookies.set('inputs', inputs);
  console.log(inputs);
  return inputs;
}