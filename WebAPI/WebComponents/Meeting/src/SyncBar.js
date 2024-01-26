import React, { useState, useEffect } from 'react';
import { syncBarStyle } from './styles';
import GetVideoPosition from './video'

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
    fetch('#--API_URL--#/editor/videosync', request)
    props.closeSyncBar()
  }

  const getCurrentVideoPosition = () => {
    let videoPosition = GetVideoPosition()
    setInputValue(convertToMMSS(videoPosition))
  }

  const convertToSec = () => {
    var array = inputValue.split(":")
    return parseInt(array[0] * 60) + parseInt(array[1])
  }

  const convertToMMSS = (seconds) => {
    const mm = Math.floor(seconds / 60).toString()
    const ss = Math.floor(seconds % 60).toString()
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
        <button style={syncBarStyle.button} onClick={() => submitSync(convertToSec())}>SAVE</button>
      </div>
  );
}

