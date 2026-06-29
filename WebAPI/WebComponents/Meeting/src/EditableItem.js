import { useLayoutEffect, useRef } from "react";
import { editableStyle } from './styles'

export default function EditableItem(props) {
    const { agendaItem, editableHTML, meetingId, language, onUpdated } = props
    const editorRef = useRef(null)

    useLayoutEffect(() => {
        editorRef.current.innerHTML = editableHTML
    }, [editableHTML])

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

    const handleBlur = () => {
        const edited = editorRef.current.innerHTML
        const editedHtml = repackHtml(edited)
        const agendaPoint = agendaItem.agendaPoint
        fetch('#--API_URL--#/editor/edit', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + localStorage.getItem("userToken")
            },
            body: JSON.stringify({
                html: editedHtml,
                decision: edited,
                meetingId,
                agendaPoint,
                language
            })
        })
        onUpdated(editedHtml)
    }

    return (
        <div
            ref={editorRef}
            contentEditable
            suppressContentEditableWarning
            tabIndex="0"
            style={editableStyle}
            onBlur={handleBlur}
        />
    )
}
