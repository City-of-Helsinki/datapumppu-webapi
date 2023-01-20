import React from 'react'
import {
  headerStyle as style
} from './styles';
export default function Header(props) {
  return (
    <div style={style.header}>
      <div style={{ flex: 3 }}>
        EDIT
      </div>
      <div style={{ flex: 1 }}>
        <button style={style.logoutBtn} onClick={props.submitLogout}>X</button>
      </div>
    </div>
  )
}


