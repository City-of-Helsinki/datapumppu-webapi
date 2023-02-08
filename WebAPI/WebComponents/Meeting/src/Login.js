import React, { useState, useEffect } from 'react'
import {
  loginStyle as style
} from './styles';
export default function Header(props) {
  const [username, setUsername] = useState("")
  const [password, setPassword] = useState("")
  const [top, setTop] = useState(0)

  useEffect(() => {
    window.addEventListener("scroll", calculateTop)
    calculateTop()
    return () => {
      window.removeEventListener("scroll", calculateTop)
    }
  }, [])

  const calculateTop = (e) => {
    const scrollTop = window.scrollY
    const top = scrollTop <= 125 ? 125 - scrollTop : 0
    console.log(top)
    setTop(top)
  }

  return (
    <div style={{
        position: "fixed",
        top: top,
        right: "0"
    }}>
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


