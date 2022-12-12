
import React, { useEffect } from 'react'
import ReactDOM from 'react-dom'

export default function Decision () {
  // Declare a new state variable, which we'll call "count"
  const [count, setCount] = React.useState(0)
  const [buttonText, setButtonText] = React.useState('decision button text')
  const message = `You clicked ${count} times`

  useEffect(() => {
    const fetchData = async () => {
      const response = await fetch('#--API_URL--#/api/buttontext')
      const json = await response.json()
      setButtonText(json.text)
    }

    fetchData()
  }, [setButtonText])

  return (
    <div>
      <p>{message}</p>
      <button type='button' onClick={() => setCount(count + 1)}>
        {buttonText}
      </button>
    </div>
  )
}

