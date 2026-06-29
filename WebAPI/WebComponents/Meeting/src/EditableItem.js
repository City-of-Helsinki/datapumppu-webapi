import { useState } from "react";
import ReactQuill from 'react-quill-new';
import 'react-quill-new/dist/quill.snow.css';
import { editableStyle } from './styles'

const TOOLBAR = [
    ['bold', 'italic', 'underline'],
    [{ list: 'ordered' }, { list: 'bullet' }],
    ['clean']
]

export default function EditableItem(props) {
    const { agendaItem, editableHTML, meetingId, language, onUpdated } = props
    const [userInput, setUserInput] = useState(false)
    const [html, setHtml] = useState(editableHTML)

    const repackHtml = (item) => {
        var div = document.createElement('div')
        var newDiv = document.createElement('div')
        div.innerHTML = agendaItem.html
        var content = div.querySelectorAll(".SisaltoSektio")[0]
        if (content) {
            const editableDiv = document.createElement('div')
            editableDiv.innerHTML = item
            newDiv.appendChild(editableDiv)
            newDiv.appendChild(content)
        } else {
            newDiv.innerHTML = item
        }
        return newDiv.innerHTML
    }

    const submitChanges = () => {
        const editedHtml = repackHtml(html)
        const agendaPoint = agendaItem.agendaPoint
        const request = {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + localStorage.getItem("userToken")
            },
            body: JSON.stringify({
                html: editedHtml,
                decision: html,
                meetingId,
                agendaPoint,
                language
            })
        }
        fetch('#--API_URL--#/editor/edit', request)
        onUpdated(editedHtml)
        setUserInput(false)
    }

    return (
        <div>
            <div tabIndex="0" onFocus={() => setUserInput(true)}>
                {userInput ?
                    <ReactQuill
                        value={html}
                        onChange={setHtml}
                        onBlur={submitChanges}
                        modules={{ toolbar: TOOLBAR }}
                        theme="snow"
                    />
                    :
                    <div
                        style={editableStyle}
                        dangerouslySetInnerHTML={{ __html: html }}
                    />
                }
            </div>
        </div>
    );
}
