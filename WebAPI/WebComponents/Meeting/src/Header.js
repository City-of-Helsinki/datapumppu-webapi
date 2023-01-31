import React from 'react'
import {
  headerStyle as style
} from './styles';
import { FaClock } from "react-icons/fa";

export default function Header(props) {
  return (
    <div style={style.header}>
      <button aria-label="sync" style={style.syncBtn} onClick={props.toggleSyncBar}><FaClock /></button>
      <div>
        EDIT
      </div>
      <div >
        <button aria-label="close" style={style.logoutBtn} onClick={props.submitLogout}>X</button>
      </div>
    </div>
  )
}


