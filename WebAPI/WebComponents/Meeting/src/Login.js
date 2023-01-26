import React, { useState } from 'react'
import {
  loginStyle as style
} from './styles';
export default function Header(props) {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");

  return (
    <div style={style.loginPopup}>
      <div style={style.loginForm}>
        <div style={style.label}>
          <label>USERNAME:</label>
        </div>
        <div>
          <input style={style.input} id="username" value={username} type='text' onChange={(e) => setUsername(e.target.value)} />
        </div>
        <div style={style.label}>
          <label>PASSWORD:</label>
        </div>
        <div>
          <input style={style.input} id="password" value={password} type="password" onChange={(e) => setPassword(e.target.value)} />
        </div>
        <div style={style.buttonRow}>
          <button style={style.button} onClick={props.closeLogin}>CANCEL</button>
          <button style={style.button} onClick={() => props.submitLogin(username, password)}>OK</button>
        </div>
      </div>
    </div>
  )
}


