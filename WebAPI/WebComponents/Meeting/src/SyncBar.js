import React, { useState } from 'react';

export default function SyncBar(props) {
  const [inputValue, setInputValue] = useState('');
  const { meetingId } = props

  const submitSync = (videoposition) => {

    const request = {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer ' + localStorage.getItem("userToken")
      },
      body: JSON.stringify({
        videoposition,
        meetingId
      })
    }
    fetch('#--API_URL--#/editor/videosync', request)
    props.closeSyncBar()
  }


  const convertToSec = () => {
    var array = inputValue.split(":")
    return parseInt(array[0] * 60) + parseInt(array[1])
  }

  return (
    <div style={{ position: 'fixed', bottom: 0, left: 0, right: 0, background: 'black' }}>
      <div style={{ alignItems: 'left', padding: '10px', color: "white" }}>
        VIDEO POSITION AT THE START OF AGENDA 2:
        <input
          value={inputValue}
          onChange={e => setInputValue(e.target.value)}
          placeholder="0:00"
          style={{ margin: '5px', padding: '5px', width: "50px" }}
        />
        <button style={{ padding: '5px' }}>SYNC</button>
        <button style={{ padding: '5px' }} onClick={() => submitSync(convertToSec(inputValue))}>SAVE</button>
      </div>
    </div>
  );
}

