import React, { useState } from 'react';
import { syncBarStyle } from './styles';
export default function SyncBar(props) {
  const [inputValue, setInputValue] = useState('');
  const { meetingId, agendaPointTimestamp } = props

  const submitSync = (videoposition) => {
    const request = {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer ' + localStorage.getItem("userToken")
      },
      body: JSON.stringify({
        "MeetingID": meetingId,
        "Timestamp": agendaPointTimestamp,
        "VideoPosition": videoposition
      })
    }
    fetch('http://localhost:5212/editor/videosync', request)
    props.closeSyncBar()
  }

  const getCurrentVideoPosition = () => {
    const videoPosition = 0;
    setInputValue(convertToMMSS(videoPosition))
  }

  const convertToSec = () => {
    var array = inputValue.split(":")
    return parseInt(array[0] * 60) + parseInt(array[1])
  }

  const convertToMMSS = (seconds) => {
    const mm = Math.floor(seconds / 60)
    const ss = Math.floor(seconds % 60)
    return mm + ":" + (ss.length < 2 ? "0" + ss : ss)
  }

  return (
    <div style={syncBarStyle.footer}>
        VIDEO POSITION AT THE START OF AGENDA 2:
        <input
          value={inputValue}
          onChange={e => setInputValue(e.target.value)}
          placeholder="0:00"
          style={syncBarStyle.input}
        />
        <button style={syncBarStyle.button} onClick={() => getCurrentVideoPosition()}>SYNC</button>
        <button style={syncBarStyle.button} onClick={() => submitSync(convertToSec(inputValue))}>SAVE</button>
      </div>
  );
}

