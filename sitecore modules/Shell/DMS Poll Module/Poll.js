if(DMSPoll == undefined)
{
  var DMSPoll  = new Object();
}

DMSPoll.showPlaceVoteBtn = false;
DMSPoll.showBackToVoteBtn = false;
DMSPoll.ReceiveServerData = function (resultFromServer)
{
   DMSPoll.OnReceiveServerData(resultFromServer);
};

DMSPoll.OnReceiveServerData = function (resultFromServer)
{
    document.getElementById(this.divContent).innerHTML = resultFromServer;
    document.getElementById(this.placeVote).style.display = this.showPlaceVoteBtn ? "" : "none";
};

DMSPoll.RaiseCallback = function (actionID)
{
    var pollItemOptions = document.getElementsByName(this.clientID);
    var checkedOptionId = "";
    
    for (i=0; i<pollItemOptions.length; i++)
    {
        if (pollItemOptions[i].checked == true)
        {
            checkedOptionId = pollItemOptions[i].value;
        }
    }
    if (checkedOptionId == "")
    {
        alert(this.alertText);
        return;
    }
    this.showPlaceVoteBtn = false;
    this.CallServer(checkedOptionId);       
};

//Cookie check
DMSPoll.CheckCookie = function() {   
    if (document.cookie == "") {        
        document.getElementById(this.divContent).style.display = "none";
        document.getElementById(this.placeVote).style.display = "none";
        document.getElementById("divCookiesRequired").style.display = "block";
    }    
}

/* Set a cookie to be sure that one exists. */
document.cookie = 'cookie_enabled=true;' + document.cookie;


