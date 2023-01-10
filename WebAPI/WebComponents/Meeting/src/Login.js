import React, { useState } from 'react'
import './theme.css'

export default function Header(props) {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");

  const style = {
    loginPopup: {
      transition: "all ease .2s",
      position: "fixed",
      top: "0%",
      right: "0%"
    },
    loginForm: {
      position: "absolute",
      top: "0px",
      right: "0px",
      width: "300px",
      backgroundColor: "#333333",
      padding: "10px",
      color: "#ffffff",
    },
    label: {
      margin: "15px 10px 8px",
    },
    input: {
      width: "100%",
      marginBottom: "10px",
      padding: "10px 5px",
      color: "black",
    },
    buttonRow: {
      marginTop: "10px",
      paddingTop: "10px",
      textAlign: "right",
    },
    button:  {
      padding: "10px",
      color: "black",
    } 
  }

  return (
    <div style={style.loginPopup}>
      <div style={style.loginForm}>
        <div>
          <label style={style.label}>USERNAME:</label>
        </div>
        <div>
          <input style={style.input} value={username} type='text' onChange={(e) => setUsername(e.target.value)} />
        </div>
        <div>
          <label style={style.label}>PASSWORD:</label>
        </div>
        <div>
          <input style={style.input} value={password} type="password" onChange={(e) => setPassword(e.target.value)} />
        </div>
        <div style={style.buttonRow}>
          <button style={style.button} onClick={props.closeLogin}>CANCEL</button>
          <button style={style.button} onClick={() => props.submitLogin(username, password)}>OK</button>
        </div>
      </div>
    </div>
  )
}


