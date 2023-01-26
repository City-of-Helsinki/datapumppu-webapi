import React from "react"
import ReactDom from "react-dom/client"
import Meeting from "./Meeting"
import i18n from './i18n';
import { I18nextProvider} from 'react-i18next'

// import './theme.css'

const root = ReactDom.createRoot(document.getElementById('meeting'))
root.render(
    <I18nextProvider i18n={i18n}>
        <Meeting />
    </I18nextProvider>

)