const BASE_URL = import.meta.env.VITE_API_URL

async function handleResponse(response) {
  if (!response.ok) {
    const body = await response.json().catch(() => ({ error: 'An unexpected error occurred.' }))
    throw new Error(body.error || 'An unexpected error occurred.')
  }
  return response.json()
}

export async function createSubscription(data) {
  const response = await fetch(`${BASE_URL}/subscriptions`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(data)
  })
  return handleResponse(response)
}

export async function getWeather(email) {
  const response = await fetch(`${BASE_URL}/subscriptions/${encodeURIComponent(email)}/weather`)
  return handleResponse(response)
}
