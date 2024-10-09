<script>
	// Variables to hold message and color
	let message = '';
	let color = '#ffffff'; // Default color: white

    // @ts-ignore
	function hexToRgb(hex) {
		hex = hex.replace(/^#/, '');
		let bigint = parseInt(hex, 16);
		let r = (bigint >> 16) & 255;
		let g = (bigint >> 8) & 255;
		let b = bigint & 255;
		return { r, g, b };
	}

	// Testing sending message to pi
	async function sendMessage() {
		if (!message) {
			alert('Message cannot be empty!');
			return;
		}
        // Hex to rgb
        const rgbColor = hexToRgb(color);
		try {
			// Send message and color to backend API
			const response = await fetch('https://localhost:7033/api/RemoteAPI/DisplayMessage', {
				method: 'POST',
				headers: {
					'Content-Type': 'application/json'
				},
				body: JSON.stringify({
					message: message,
					color: rgbColor
				})
			});

			if (response.ok) {
				console.log('Message sent successfully');
			} else {
				console.error('Failed to send message');
			}
		} catch (error) {
			console.error('Error sending message:', error);
		}
	}
</script>

<!-- UI Elements -->
<div>
	<h1>MatrixRemote</h1>

	<!-- Message input -->
	<label for="message">Type your message:</label>
	<textarea id="message" bind:value={message} placeholder="Enter your message"></textarea>

	<!-- Color picker -->
	<label for="color">Choose a color:</label>
	<input type="color" id="color" bind:value={color} />

	<!-- Send button -->
	<button on:click={sendMessage}>Send to Matrix</button>
</div>

<style>
	div {
		display: flex;
		flex-direction: column;
		max-width: 400px;
		margin: auto;
	}

	textarea {
		margin-bottom: 10px;
		padding: 5px;
	}

	button {
		padding: 10px;
		background-color: #0070f3;
		color: white;
		border: none;
		border-radius: 5px;
		cursor: pointer;
	}
</style>
